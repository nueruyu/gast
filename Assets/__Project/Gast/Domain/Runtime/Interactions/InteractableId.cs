using System;

namespace Gast.Domain.Interactions
{
    /// <summary>
    /// Unique identifier for an interactable instance.
    /// Uses the Unity object's instance ID for performance.
    /// </summary>
    public readonly struct InteractableId : IEquatable<InteractableId>
    {
        readonly int value;

        InteractableId(int value)
        {
            this.value = value;
        }

        public static InteractableId FromInstanceId(int instanceId) => new(instanceId);

        public int ToInt() => value;

        public bool Equals(InteractableId other) => value == other.value;

        public override bool Equals(object obj) => obj is InteractableId other && Equals(other);

        public override int GetHashCode() => value;

        public override string ToString() => value.ToString();

        public static bool operator ==(InteractableId left, InteractableId right) => left.Equals(right);

        public static bool operator !=(InteractableId left, InteractableId right) => !left.Equals(right);
    }
}
