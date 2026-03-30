using UnityEngine;

namespace Gast.Unity.Shared.Animations
{
    /// <summary>
    /// Type-safe reference to an Animator trigger parameter.
    /// Create one asset per trigger name; assign to action settings to
    /// drive which Animator trigger is fired without hardcoding strings.
    /// </summary>
    [CreateAssetMenu(fileName = "AnimatorTriggerSymbol", menuName = "Gast/Animation/Trigger Symbol")]
    public class AnimatorTriggerSymbol : ScriptableObject
    {
        [SerializeField]
        string triggerName;

        // Lazily cached; computed on first access so the asset needs no OnEnable.
        int? hash;
        public int Hash => hash ??= Animator.StringToHash(triggerName);
    }
}
