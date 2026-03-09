using System;
using UnityEngine;

namespace Gast.Core.Values
{
    /// <summary>
    /// Represents a rate or probability, clamped between 0 and 1.
    /// </summary>
    [Serializable]
    public struct Rate : IEquatable<Rate>
    {
        [SerializeField]
        float value;

        public float Value => value;

        public Rate(float value)
        {
            this.value = Mathf.Clamp01(value);
        }

        public static implicit operator float(Rate rate) => rate.value;
        public static implicit operator Rate(float value) => new(value);

        public bool Equals(Rate other) => value.Equals(other.value);
        public override bool Equals(object obj) => obj is Rate other && Equals(other);
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString("P0");
    }
}