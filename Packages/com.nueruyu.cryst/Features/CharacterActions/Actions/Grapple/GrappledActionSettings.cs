using System;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Characters.IK;
using Gast.Unity.Shared.Animations;
using Gast.Unity.Shared.Attachments;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    [CreateAssetMenu(fileName = "GrappledActionSettings", menuName = "Gast/Actions/Grappled Action Settings")]
    public class GrappledActionSettings : CharacterActionSettings
    {
        [Header("Core")]
        [SerializeField]
        float duration = 1.5f;

        [Header("Animation")]
        [SerializeField]
        AnimatorTriggerSymbol grappledTrigger;

        [Header("IK")]
        [Tooltip("IK goals to activate on this character (victim), each mapped to an anchor on the attacker.")]
        [SerializeField]
        IKBinding[] ikBindings = Array.Empty<IKBinding>();

        public float Duration => duration;
        public AnimatorTriggerSymbol GrappledTrigger => grappledTrigger;
        public IKBinding[] IKBindings => ikBindings;

        [Serializable]
        public class IKBinding
        {
            [Tooltip("IK goal on this character (victim) to activate.")]
            public AvatarIKGoal SelfGoal;

            [Tooltip("Anchor on the attacker whose transform is used as the IK target.")]
            public AttachmentAnchorSymbol AttackerAnchor;
        }
    }
}
