using HwigiTower.Combat;
using HwigiTower.Core;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace HwigiTower.Training
{
    public sealed class CombatTrainingAgent : Agent
    {
        private const int PlayerMaxHp = 24;
        private const int PlayerAttack = 6;
        private const int EnemyMaxHp = 20;
        private const int EnemyAttack = 5;
        private const int SkillDamage = 8;
        private const int MaxDecisionSteps = 12;

        private CombatantState _player;
        private CombatantState _enemy;
        private CombatController _combat;
        private int _episodeIndex;
        private int _stepIndex;
        private CombatAction _lastAction;

        public override void Initialize()
        {
            ResetEpisodeState();
        }

        public override void OnEpisodeBegin()
        {
            ResetEpisodeState();
        }

        private void FixedUpdate()
        {
            RequestDecision();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(NormalizeHp(_player.Hp, _player.MaxHp));
            sensor.AddObservation(NormalizeHp(_enemy.Hp, _enemy.MaxHp));
            sensor.AddObservation(PlayerAttack / 20f);
            sensor.AddObservation(EnemyAttack / 20f);
            sensor.AddObservation(_lastAction == CombatAction.Attack ? 1f : 0f);
            sensor.AddObservation(_lastAction == CombatAction.Defend ? 1f : 0f);
            sensor.AddObservation(_lastAction == CombatAction.Skill ? 1f : 0f);
            sensor.AddObservation(_stepIndex / (float)MaxDecisionSteps);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var selectedAction = ResolveAction(actions.DiscreteActions[0]);
            var playerHpBefore = _player.Hp;
            var enemyHpBefore = _enemy.Hp;
            var result = _combat.ResolveRound(
                _player,
                _enemy,
                selectedAction,
                null,
                selectedAction == CombatAction.Skill ? SkillDamage : null);

            var enemyDamage = Mathf.Max(0, enemyHpBefore - _enemy.Hp);
            var playerDamage = Mathf.Max(0, playerHpBefore - _player.Hp);

            AddReward(enemyDamage * 0.05f);
            AddReward(playerDamage * -0.03f);
            AddReward(selectedAction == CombatAction.Defend && result.PlayerDamagePrevented > 0 ? 0.05f : 0f);
            AddReward(-0.01f);

            _lastAction = selectedAction;
            _stepIndex++;

            if (result.EnemyDefeated)
            {
                AddReward(1f);
                EndEpisode();
                return;
            }

            if (result.PlayerDefeated)
            {
                AddReward(-1f);
                EndEpisode();
                return;
            }

            if (_stepIndex >= MaxDecisionSteps)
            {
                AddReward(-0.25f);
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discrete = actionsOut.DiscreteActions;
            discrete[0] = _enemy.Hp <= SkillDamage ? 2 : _player.Hp <= 8 ? 1 : 0;
        }

        private void ResetEpisodeState()
        {
            var context = new DeterministicRunContext("training-combat", 7301 + _episodeIndex);
            _combat = new CombatController(context, "mlagents.training." + _episodeIndex);
            _player = new CombatantState("training.player", PlayerMaxHp, PlayerAttack);
            _enemy = new CombatantState("training.enemy", EnemyMaxHp, EnemyAttack);
            _stepIndex = 0;
            _lastAction = CombatAction.Attack;
            _episodeIndex++;
        }

        private static CombatAction ResolveAction(int action)
        {
            return action switch
            {
                1 => CombatAction.Defend,
                2 => CombatAction.Skill,
                _ => CombatAction.Attack
            };
        }

        private static float NormalizeHp(int hp, int maxHp)
        {
            return maxHp <= 0 ? 0f : Mathf.Clamp01(hp / (float)maxHp);
        }
    }
}
