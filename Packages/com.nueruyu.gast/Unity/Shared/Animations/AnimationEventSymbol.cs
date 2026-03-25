using UnityEngine;

namespace Gast.Unity.Shared.Animations
{
    /// <summary>
    /// Represents a unique symbol for an animation event, allowing for type-safe referencing.
    /// Create this as a ScriptableObject asset to define a new event type.
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationEventSymbol", menuName = "Gast/Animation/Event Symbol")]
    public class AnimationEventSymbol : ScriptableObject
    {
    }
}
