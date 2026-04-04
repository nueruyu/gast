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

            references.Constraint.weight = weight;

            if (references.Target != null && target != null)
            {
                references.Target.position = target.position;
                references.Target.rotation = target.rotation;
            }
        }

        public void SetGoalReferences(AvatarIKGoal goal, IKGoalReferences references)
        {
            switch (goal)
            {
                case AvatarIKGoal.RightHand: rightHand = references; break;
                case AvatarIKGoal.LeftHand:  leftHand  = references; break;
                case AvatarIKGoal.RightFoot: rightFoot = references; break;
                case AvatarIKGoal.LeftFoot:  leftFoot  = references; break;
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
