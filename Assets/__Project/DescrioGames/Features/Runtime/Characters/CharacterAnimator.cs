using UnityEngine;

namespace DescrioGames.Features.Characters
{
    /// <summary>
    /// Controls character animations through Unity's Animator component.
    /// Adapts logical commands (attack, dash, guard) into animation parameter updates.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        float dampTime = 0.1f;

        Animator animator;

        // Cached parameter IDs for performance

        readonly int walkSpeedHash = Animator.StringToHash("WalkSpeed");
        readonly int attackHash = Animator.StringToHash("Attack");
        readonly int dashHash = Animator.StringToHash("Dash");
        readonly int guardHash = Animator.StringToHash("IsGuarding");
        readonly int hitHash = Animator.StringToHash("Hit");
        readonly int deadHash = Animator.StringToHash("IsDead");

        void Awake()
        {
            TryGetComponent(out animator);
        }

        // === Public Animation Control Methods ===

        /// <summary>
        /// Set the movement speed parameter (normalized 0.0 ~ 1.0).
        /// </summary>
        public void SetMoveSpeed(float normalizedSpeed)
        {
            if (!isActiveAndEnabled) return;
            animator.SetFloat(walkSpeedHash, normalizedSpeed, dampTime, Time.deltaTime);
        }

        /// <summary>
        /// Trigger the Attack animation.
        /// </summary>
        public void PlayAttack()
        {
            if (!isActiveAndEnabled) return;
            animator.SetTrigger(attackHash);
        }

        /// <summary>
        /// Trigger the Dash animation.
        /// </summary>
        public void PlayDash()
        {
            if (!isActiveAndEnabled) return;
            animator.SetTrigger(dashHash);
        }

        /// <summary>
        /// Set the guard state (starts or stops guard animation loop).
        /// </summary>
        public void SetGuard(bool active)
        {
            if (!isActiveAndEnabled) return;
            animator.SetBool(guardHash, active);
        }

        /// <summary>
        /// Trigger the Hit reaction animation.
        /// </summary>
        public void PlayHit()
        {
            if (!isActiveAndEnabled) return;
            animator.SetTrigger(hitHash);
        }

        /// <summary>
        /// Set the dead state (triggers death animation and disables character).
        /// </summary>
        public void SetDead(bool isDead)
        {
            if (!isActiveAndEnabled) return;
            animator.SetBool(deadHash, isDead);
        }
    }
}