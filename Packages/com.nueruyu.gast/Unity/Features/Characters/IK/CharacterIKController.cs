using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Gast.Unity.Features.Characters.IK
{
    public class CharacterIKController : MonoBehaviour
    {
        [SerializeField]
        IKGoalReferences rightHand;

        [SerializeField]
        IKGoalReferences leftHand;

        [SerializeField]
        IKGoalReferences rightFoot;

        [SerializeField]
        IKGoalReferences leftFoot;

        public void SetIKTarget(AvatarIKGoal goal, Transform target, float weight)
        {
            var references = GetReferences(goal);
            if (references?.Constraint == null) return;

            var rig = references.Constraint.rig;
            if(rig == null) return;

            rig.weight = weight;

            if (references.Target != null)
            {
                if (target != null)
                {
                    references.Target.position = target.position;
                    references.Target.rotation = target.rotation;
                }
            }
        }

        IKGoalReferences GetReferences(AvatarIKGoal goal)
        {
            return goal switch
            {
                AvatarIKGoal.RightHand => rightHand,
                AvatarIKGoal.LeftHand => leftHand,
                AvatarIKGoal.RightFoot => rightFoot,
                AvatarIKGoal.LeftFoot => leftFoot,
                _ => null
            };
        }

        [Serializable]
        public class IKGoalReferences
        {
            public TwoBoneIKConstraint Constraint;
            public Transform Target;
            public Transform Hint;
        }
    }
}
