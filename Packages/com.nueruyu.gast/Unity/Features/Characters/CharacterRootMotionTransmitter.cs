using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    [RequireComponent(typeof(Animator))]
    public class CharacterRootMotionTransmitter : MonoBehaviour
    {
        Transform rootTransform;
        Animator animator;

        void Start()
        {
            rootTransform = transform.parent;
            animator = GetComponent<Animator>();
        }

        void OnAnimatorMove()
        {
            if (animator.applyRootMotion)
            {
                rootTransform.position += animator.deltaPosition;
                rootTransform.rotation *= animator.deltaRotation;
            }
        }
    }
}
