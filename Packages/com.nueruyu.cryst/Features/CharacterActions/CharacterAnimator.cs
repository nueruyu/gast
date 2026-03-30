using Gast.Unity.Shared.Animations;
using UnityEngine;


namespace Cryst.Features.CharacterActions
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

        AnimationEventReceiver animationEventReceiver;

        public AnimationEventReceiver AnimationEventReceiver => animationEventReceiver;

        void Awake()
        {
            TryGetComponent(out animator);
            TryGetComponent(out animationEventReceiver);

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

        public void PlayTrigger(AnimatorTriggerSymbol symbol)
        {
            if (!isActiveAndEnabled || symbol == null) return;
            animator.SetTrigger(symbol.Hash);
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