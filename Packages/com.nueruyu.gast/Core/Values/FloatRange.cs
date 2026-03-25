using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Core.Values
{
    /// <summary>
    /// Represents a range of float values.
    /// </summary>
    [Serializable]
    public struct FloatRange : IEquatable<FloatRange>
    {
        [SerializeField]
        float min;

        [SerializeField]
        float max;

        public float Min => min;
        public float Max => max;

        public FloatRange(
            float min,
            float max)
        {
            this.min = Mathf.Min(min, max);
            this.max = Mathf.Max(min, max);
        }

        /// <summary>
        /// Returns a random value within the range [Min, Max].
        /// </summary>
        public float Sample() => Random.Range(min, max);

        public static implicit operator FloatRange(float value) => new(value, value);

        public bool Equals(FloatRange other) => min.Equals(other.min) && max.Equals(other.max);
        public override bool Equals(object obj) => obj is FloatRange other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(min, max);

        public override string ToString()
        {
            return Mathf.Approximately(min, max) ? min.ToString("F1") : $"[{min:F1}..{max:F1}]";
        }
    }
}