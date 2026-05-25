using HwigiTower.Combat;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace HwigiTower.Training
{
    public sealed class CombatTrainingAgent : Agent
    {
        private CombatantState _player;
        private CombatantState _enemy;
        private HwigiTower.Core.DeterministicRandom _random;
        private int _episodeIndex;
        private int _stepIndex;
        private CombatAction _lastAction;
        private int _skillCooldownRemaining;
        private bool _tempoAttackReady;
        private int _attackCount;
        private int _defendCount;
        private int _skillCount;
        private int _skillBlockedCount;
        private int _skillWastedCount;
        private int _defendAttemptCount;
        private int _defendSuccessCount;
        private int _tempoBonusTriggeredCount;

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
            sensor.AddObservation(CombatTrainingExp04Rules.NormalizeHp(_player.Hp, _player.MaxHp));
            sensor.AddObservation(CombatTrainingExp04Rules.NormalizeHp(_enemy.Hp, _enemy.MaxHp));
            sensor.AddObservation(CombatTrainingExp04Rules.PlayerAttack / 20f);
            sensor.AddObservation(CombatTrainingExp04Rules.EnemyAttack / 20f);
            sensor.AddObservation(_lastAction == CombatAction.Attack ? 1f : 0f);
            sensor.AddObservation(_lastAction == CombatAction.Defend ? 1f : 0f);
            sensor.AddObservation(_lastAction == CombatAction.Skill ? 1f : 0f);
            sensor.AddObservation(_stepIndex / (float)CombatTrainingExp04Rules.MaxDecisionSteps);
            sensor.AddObservation(_skillCooldownRemaining / (float)CombatTrainingExp04Rules.SkillCooldownTurns);
            sensor.AddObservation(CombatTrainingExp04Rules.IsEnemyThreatHigh(_stepIndex) ? 1f : 0f);
            sensor.AddObservation(_tempoAttackReady ? 1f : 0f);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            if (!CombatTrainingExp04Rules.CanUseSkill(_skillCooldownRemaining))
            {
                actionMask.SetActionEnabled(0, 2, false);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var requestedAction = CombatTrainingExp04Rules.ResolveAction(actions.DiscreteActions[0]);
            var result = CombatTrainingExp04Rules.ResolveRound(
                _player,
                _enemy,
                requestedAction,
                _skillCooldownRemaining,
                _tempoAttackReady,
                _stepIndex,
                _random);

            TrackAction(result.Action);
            _skillBlockedCount += result.SkillBlocked ? 1 : 0;
            _skillWastedCount += result.SkillWasted ? 1 : 0;
            _defendAttemptCount += result.Action == CombatAction.Defend ? 1 : 0;
            _defendSuccessCount += result.DefendSucceeded ? 1 : 0;
            _tempoBonusTriggeredCount += result.TempoBonusTriggered ? 1 : 0;

            AddReward(CombatTrainingExp04Rules.CalculateStepReward(result));
            RecordStepStats(result);

            _lastAction = result.Action;
            _skillCooldownRemaining = CombatTrainingExp04Rules.NextSkillCooldown(_skillCooldownRemaining, result.Action);
            _tempoAttackReady = CombatTrainingExp04Rules.NextTempoAttackReady(_tempoAttackReady, result);
            _stepIndex++;

            if (result.EnemyDefeated)
            {
                AddReward(CombatTrainingExp04Rules.WinReward);
                RecordEpisodeStats(1f, 0f, 0f);
                EndEpisode();
                return;
            }

            if (result.PlayerDefeated)
            {
                AddReward(CombatTrainingExp04Rules.LossPenalty);
                RecordEpisodeStats(0f, 1f, 0f);
                EndEpisode();
                return;
            }

            if (_stepIndex >= CombatTrainingExp04Rules.MaxDecisionSteps)
            {
                AddReward(CombatTrainingExp04Rules.TimeoutPenalty);
                RecordEpisodeStats(0f, 0f, 1f);
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discrete = actionsOut.DiscreteActions;
            discrete[0] = CombatTrainingExp04Rules.IsEnemyThreatHigh(_stepIndex) || _player.Hp <= 8
                ? 1
                : _enemy.Hp >= CombatTrainingExp04Rules.SkillWasteHpThreshold + 4 &&
                  CombatTrainingExp04Rules.CanUseSkill(_skillCooldownRemaining) ? 2 : 0;
        }

        private void ResetEpisodeState()
        {
            _random = new HwigiTower.Core.DeterministicRandom(
                HwigiTower.Core.DeterministicSeed.Combine(7301 + _episodeIndex, "mlagents.training." + _episodeIndex));
            _player = new CombatantState("training.player", CombatTrainingExp04Rules.PlayerMaxHp, CombatTrainingExp04Rules.PlayerAttack);
            _enemy = new CombatantState("training.enemy", CombatTrainingExp04Rules.EnemyMaxHp, CombatTrainingExp04Rules.EnemyAttack);
            _stepIndex = 0;
            _lastAction = CombatAction.Attack;
            _skillCooldownRemaining = 0;
            _tempoAttackReady = false;
            _attackCount = 0;
            _defendCount = 0;
            _skillCount = 0;
            _skillBlockedCount = 0;
            _skillWastedCount = 0;
            _defendAttemptCount = 0;
            _defendSuccessCount = 0;
            _tempoBonusTriggeredCount = 0;
            _episodeIndex++;
        }

        private void TrackAction(CombatAction action)
        {
            switch (action)
            {
                case CombatAction.Defend:
                    _defendCount++;
                    break;
                case CombatAction.Skill:
                    _skillCount++;
                    break;
                default:
                    _attackCount++;
                    break;
            }
        }

        private void RecordStepStats(CombatTrainingExp04RoundResult result)
        {
            var recorder = Academy.Instance.StatsRecorder;
            var action = result.Action;
            recorder.Add("HwigiTraining/ActionAttack", action == CombatAction.Attack ? 1f : 0f);
            recorder.Add("HwigiTraining/ActionDefend", action == CombatAction.Defend ? 1f : 0f);
            recorder.Add("HwigiTraining/ActionSkill", action == CombatAction.Skill ? 1f : 0f);
            recorder.Add("HwigiTraining/SkillReady", CombatTrainingExp04Rules.CanUseSkill(_skillCooldownRemaining) ? 1f : 0f);
            recorder.Add("HwigiTraining/SkillBlocked", result.SkillBlocked ? 1f : 0f);
            recorder.Add("HwigiTraining/SkillWasted", result.SkillWasted ? 1f : 0f);
            recorder.Add("HwigiTraining/DefendSucceeded", result.DefendSucceeded ? 1f : 0f);
            recorder.Add("HwigiTraining/TempoBonusTriggered", result.TempoBonusTriggered ? 1f : 0f);
            recorder.Add("HwigiTraining/EnemyThreatHigh", CombatTrainingExp04Rules.IsEnemyThreatHigh(_stepIndex) ? 1f : 0f);
            recorder.Add("HwigiTraining/EnemyDamagePerStep", result.EnemyDamage);
            recorder.Add("HwigiTraining/PlayerDamagePerStep", result.PlayerDamage);
            recorder.Add("HwigiTraining/DamagePreventedPerStep", result.DamagePrevented);
        }

        private void RecordEpisodeStats(float win, float loss, float timeout)
        {
            var recorder = Academy.Instance.StatsRecorder;
            var totalActions = Mathf.Max(1, _attackCount + _defendCount + _skillCount);
            var actionTypesUsed = (_attackCount > 0 ? 1 : 0) + (_defendCount > 0 ? 1 : 0) + (_skillCount > 0 ? 1 : 0);
            recorder.Add("HwigiTraining/Win", win);
            recorder.Add("HwigiTraining/Loss", loss);
            recorder.Add("HwigiTraining/Timeout", timeout);
            recorder.Add("HwigiTraining/EpisodeLength", _stepIndex + 1);
            recorder.Add("HwigiTraining/FinalPlayerHpRatio", CombatTrainingExp04Rules.NormalizeHp(_player.Hp, _player.MaxHp));
            recorder.Add("HwigiTraining/FinalEnemyHpRatio", CombatTrainingExp04Rules.NormalizeHp(_enemy.Hp, _enemy.MaxHp));
            recorder.Add("HwigiTraining/EpisodeAttackShare", _attackCount / (float)totalActions);
            recorder.Add("HwigiTraining/EpisodeDefendShare", _defendCount / (float)totalActions);
            recorder.Add("HwigiTraining/EpisodeSkillShare", _skillCount / (float)totalActions);
            recorder.Add("HwigiTraining/EpisodeSkillBlockedShare", _skillBlockedCount / (float)totalActions);
            recorder.Add("HwigiTraining/SkillWastedPerEpisode", _skillWastedCount);
            recorder.Add("HwigiTraining/DefendSuccessRate", _defendSuccessCount / (float)Mathf.Max(1, _defendAttemptCount));
            recorder.Add("HwigiTraining/TempoBonusTriggeredPerEpisode", _tempoBonusTriggeredCount);
            recorder.Add("HwigiTraining/ActionDiversity", actionTypesUsed / 3f);
        }
    }
}
