using System;

namespace HwigiTower.Combat
{
    public enum MataiosActionTarget
    {
        None,
        Player,
        Enemy
    }

    public readonly struct MataiosActionPlan : IEquatable<MataiosActionPlan>
    {
        public MataiosActionPlan(
            string actionId,
            string payloadKey,
            MataiosActionTarget target,
            string reasonKey,
            bool consumesTempo,
            params string[] metricTags)
        {
            ActionId = actionId ?? string.Empty;
            PayloadKey = payloadKey ?? string.Empty;
            Target = target;
            ReasonKey = reasonKey ?? string.Empty;
            ConsumesTempo = consumesTempo;
            MetricTags = metricTags ?? Array.Empty<string>();
        }

        public string ActionId { get; }
        public string PayloadKey { get; }
        public MataiosActionTarget Target { get; }
        public string ReasonKey { get; }
        public bool ConsumesTempo { get; }
        public string[] MetricTags { get; }
        public bool IsNone => string.IsNullOrEmpty(ActionId) || ActionId == MataiosCombatBrain.ActionNoneDown;

        public bool Equals(MataiosActionPlan other)
        {
            return ActionId == other.ActionId &&
                PayloadKey == other.PayloadKey &&
                Target == other.Target &&
                ReasonKey == other.ReasonKey &&
                ConsumesTempo == other.ConsumesTempo &&
                MetricTagsEqual(MetricTags, other.MetricTags);
        }

        public override bool Equals(object obj)
        {
            return obj is MataiosActionPlan other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + ActionId.GetHashCode();
                hash = hash * 31 + PayloadKey.GetHashCode();
                hash = hash * 31 + Target.GetHashCode();
                hash = hash * 31 + ReasonKey.GetHashCode();
                hash = hash * 31 + ConsumesTempo.GetHashCode();
                for (var i = 0; i < MetricTags.Length; i++)
                {
                    hash = hash * 31 + MetricTags[i].GetHashCode();
                }

                return hash;
            }
        }

        private static bool MetricTagsEqual(string[] first, string[] second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first == null || second == null || first.Length != second.Length)
            {
                return false;
            }

            for (var i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
