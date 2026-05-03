using System.Collections.Generic;
using HwigiTower.Abilities;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.LLM;
using HwigiTower.NPC;

namespace HwigiTower.Run
{
    public sealed class PrototypeRunState
    {
        private const int BasePlayerMaxHp = 24;
        private const int BasePlayerAttack = 5;
        private const int FallbackEnemyHp = 12;
        private const int FallbackEnemyAttack = 3;
        private const int MinMental = -100;
        private const int MaxMental = 100;
        private const int MinGold = 0;
        private const int MaxGold = 999;
        private const int MinGlitchLevel = 0;
        private const int MaxGlitchLevel = 100;
        private const int MinAffinity = -100;
        private const int MaxAffinity = 100;
        private const int PrototypeShopAbilityCost = 10;

        private readonly GameFlowEventBus _eventBus;
        private readonly ReflectionPipeline _reflectionPipeline;
        private readonly SynergyDetector _synergyDetector;
        private readonly List<SynergyState> _activeSynergies = new List<SynergyState>();
        private bool _runCompleted;
        private int _playerHp = BasePlayerMaxHp;
        private int _playerMaxHp = BasePlayerMaxHp;
        private int _playerAttack = BasePlayerAttack;
        private int _mental;
        private int _gold;
        private int _glitchLevel;
        private int _affinity;

        public PrototypeRunState(string runId, GameFlowEventBus eventBus)
        {
            RunId = runId ?? string.Empty;
            _eventBus = eventBus;
            MemoryRepo = new InMemoryNpcMemoryRepo();
            LLMProvider = new CachedLLMProvider(MemoryRepo, new OnDeviceLLMProvider(100, new DeterministicFakeLLMProvider()));
            _reflectionPipeline = new ReflectionPipeline(MemoryRepo, LLMProvider, eventBus);
            _synergyDetector = new SynergyDetector(eventBus, RunId);
            Abilities = new AbilityInventory(eventBus, RunId);
        }

        public string RunId { get; }
        public AbilityInventory Abilities { get; }
        public INPCMemoryRepo MemoryRepo { get; }
        public ILLMProvider LLMProvider { get; }
        public NpcStateMachine NpcStateMachine { get; private set; }
        public int NodesResolved { get; private set; }
        public int BattlesWon { get; private set; }
        public bool RunCompleted => _runCompleted;
        public int PlayerHp => _playerHp;
        public int PlayerMaxHp => _playerMaxHp;
        public int PlayerAttack => _playerAttack;
        public int Mental => _mental;
        public int Gold => _gold;
        public int GlitchLevel => _glitchLevel;
        public int Affinity => _affinity;

        public PrototypeRunSnapshot CreateSnapshot()
        {
            return new PrototypeRunSnapshot(
                RunId,
                _playerHp,
                _playerMaxHp,
                _playerAttack,
                _mental,
                _gold,
                _glitchLevel,
                _affinity,
                NodesResolved,
                BattlesWon,
                Abilities.Abilities.Count,
                _runCompleted);
        }

        public int ModifyMental(int amount)
        {
            _mental = Clamp(_mental + amount, MinMental, MaxMental);
            return _mental;
        }

        public int ModifyGold(int amount)
        {
            _gold = Clamp(_gold + amount, MinGold, MaxGold);
            return _gold;
        }

        public int ModifyGlitchLevel(int amount)
        {
            _glitchLevel = Clamp(_glitchLevel + amount, MinGlitchLevel, MaxGlitchLevel);
            return _glitchLevel;
        }

        public int ModifyAffinity(int amount)
        {
            _affinity = Clamp(_affinity + amount, MinAffinity, MaxAffinity);
            return _affinity;
        }

        public void AttachNpcStateMachine(NpcStateMachine stateMachine)
        {
            NpcStateMachine = stateMachine;
        }

        public PrototypeNodeResolution ResolveBattle(DeterministicRunContext context, string nodeId, string encounterId, EnemyData enemyData, IReadOnlyList<SynergyData> trackedSynergies)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounterId, "run already completed", true);
            }

            EvaluateSynergies(trackedSynergies);
            RecalculatePlayerStats();
            var player = new CombatantState("player", _playerMaxHp, _playerAttack, _playerHp);
            var enemy = CreateEnemyState(enemyData);
            var combat = new CombatController(context, string.IsNullOrEmpty(encounterId) ? nodeId : encounterId);

            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatStarted, RunId, nodeId, enemy.Id));

            var rounds = 0;
            CombatRoundResult round;
            do
            {
                round = combat.ResolveRound(player, enemy);
                rounds++;
            }
            while (!round.IsComplete && rounds < 12);

            NodesResolved++;
            if (enemy.IsDefeated)
            {
                BattlesWon++;
            }

            _playerHp = player.Hp;
            var resultId = enemy.IsDefeated ? "victory" : "defeat";
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatCompleted, RunId, nodeId, resultId));
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, resultId));

            if (player.IsDefeated)
            {
                ApplyNpcTrigger("battle.defeat");
                CompleteRun("defeat");
            }
            else if (enemy.IsDefeated)
            {
                ApplyNpcTrigger("battle.victory");
            }

            var message = $"{resultId} | player {player.Hp}/{player.MaxHp} | enemy {enemy.Hp}/{enemy.MaxHp} | rounds {rounds}";
            return new PrototypeNodeResolution(nodeId, encounterId, message, _runCompleted);
        }

        public PrototypeNodeResolution ResolveRest(string nodeId)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, string.Empty, "run already completed", true);
            }

            NodesResolved++;
            _playerHp = _playerMaxHp;
            var recall = _reflectionPipeline.LoadRecallPrompt(RunId, 3);
            ApplyNpcTrigger("rest.recall");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "rest"));
            return new PrototypeNodeResolution(nodeId, "recall", recall, false);
        }

        public PrototypeNodeResolution ResolveShop(string nodeId, AbilityData grantedAbility, IReadOnlyList<SynergyData> trackedSynergies)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, string.Empty, "run already completed", true);
            }

            NodesResolved++;
            if (grantedAbility == null)
            {
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-empty"));
                return new PrototypeNodeResolution(nodeId, string.Empty, "shop unavailable", false);
            }

            if (_gold < PrototypeShopAbilityCost)
            {
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-insufficient-gold"));
                return new PrototypeNodeResolution(nodeId, string.Empty, $"purchase failed: gold {_gold}/{PrototypeShopAbilityCost}", false);
            }

            var added = Abilities.Add(grantedAbility);
            if (!added)
            {
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-empty"));
                return new PrototypeNodeResolution(nodeId, string.Empty, "purchase failed: ability unavailable", false);
            }

            ModifyGold(-PrototypeShopAbilityCost);
            EvaluateSynergies(trackedSynergies);
            RecalculatePlayerStats();
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-purchase-success"));
            return new PrototypeNodeResolution(nodeId, grantedAbility.Id, $"purchase success: {grantedAbility.Id}", false);
        }

        public PrototypeNodeResolution ResolveRemnant(string nodeId)
        {
            if (!_runCompleted)
            {
                NodesResolved++;
                CompleteRun("remnant");
            }

            return new PrototypeNodeResolution(nodeId, "run.completed", "run completed: reflection saved", true);
        }

        public PrototypeNodeResolution ResolveGeneric(string nodeId, string payloadId, string message)
        {
            if (!_runCompleted)
            {
                NodesResolved++;
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            }

            return new PrototypeNodeResolution(nodeId, payloadId, message, _runCompleted);
        }

        private void EvaluateSynergies(IReadOnlyList<SynergyData> trackedSynergies)
        {
            _activeSynergies.Clear();
            _activeSynergies.AddRange(_synergyDetector.Evaluate(Abilities.Abilities, trackedSynergies));
        }

        private void RecalculatePlayerStats()
        {
            var previousMaxHp = _playerMaxHp;
            var modifiers = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies);
            _playerMaxHp = System.Math.Max(1, BasePlayerMaxHp + modifiers.PlayerMaxHpBonus);
            _playerAttack = System.Math.Max(0, BasePlayerAttack + modifiers.PlayerAttackBonus);

            if (_playerMaxHp > previousMaxHp)
            {
                _playerHp += _playerMaxHp - previousMaxHp;
            }

            _playerHp = System.Math.Max(0, System.Math.Min(_playerHp, _playerMaxHp));
        }

        private void CompleteRun(string cause)
        {
            if (_runCompleted)
            {
                return;
            }

            _runCompleted = true;
            var summary = $"cause={cause};nodes={NodesResolved};battles={BattlesWon}";
            _reflectionPipeline.TrySaveReflection(RunId, summary, out _);
            ApplyNpcTrigger("run.completed");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.RunCompleted, RunId, RunId, cause));
        }

        private void ApplyNpcTrigger(string triggerId)
        {
            NpcStateMachine?.TryApply(triggerId);
        }

        private static int Clamp(int value, int min, int max)
        {
            return System.Math.Max(min, System.Math.Min(value, max));
        }

        private static CombatantState CreateEnemyState(EnemyData enemyData)
        {
            if (enemyData == null)
            {
                return new CombatantState("enemy.placeholder", FallbackEnemyHp, FallbackEnemyAttack);
            }

            return new CombatantState(enemyData.Id, enemyData.Hp, enemyData.Attack);
        }
    }
}
