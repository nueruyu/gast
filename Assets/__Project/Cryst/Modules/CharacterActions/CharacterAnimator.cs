using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        float dampTime = 0.1f;

        Animator animator;

        readonly int walkSpeedHash = Animator.StringToHash("WalkSpeed");
        readonly int attackHash = Animator.StringToHash("Attack");
        readonly int dashHash = Animator.StringToHash("Dash");
        readonly int guardHash = Animator.StringToHash("IsGuarding");
        readonly int hitHash = Animator.StringToHash("Hit");
        readonly int deadHash = Animator.StringToHash("IsDead");

        void Awake()
        {
            TryGetComponent(out animator);

            SetMoveSpeed(0);
        }

        public void SetMoveSpeed(float normalizedSpeed)
        {
            if (!isActiveAndEnabled) return;
            animator.SetFloat(walkSpeedHash, normalizedSpeed, dampTime, Time.deltaTime);
        }

        public void PlayAttack()
        {
            if (!isActiveAndEnabled) return;
            animator.SetTrigger(attackHash);
        }

        public void PlayDash()
        {
            if (!isActiveAndEnabled) return;
            animator.SetTrigger(dashHash);
        }

        public void SetGuard(bool active)
        {
            if (!isActiveAndEnabled) return;
            animator.SetBool(guardHash, active);
        }

        public void PlayHit()
        {
            if (!isActiveAndEnabled) return;
            animator.SetTrigger(hitHash);
        }

        public void SetDead(bool isDead)
        {
            if (!isActiveAndEnabled) return;
            animator.SetBool(deadHash, isDead);
        }
    }
}