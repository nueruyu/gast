using System;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Characters.IK;
using Gast.Unity.Shared.Animations;
using Gast.Unity.Shared.Attachments;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    [CreateAssetMenu(fileName = "GrappleThrowActionSettings", menuName = "Gast/Actions/Grapple Throw Action Settings")]
    public class GrappleThrowActionSettings : CharacterActionSettings
    {
        [Header("Core")]
        [SerializeField]
        float duration = 1.5f;

        [Header("Animation")]
        [SerializeField]
        AnimatorTriggerSymbol throwTrigger;

        [Header("IK")]
        [Tooltip("IK goals to activate on the VICTIM, each mapped to an anchor on the ATTACKER.")]
        [SerializeField]
        IKBinding[] ikBindings = Array.Empty<IKBinding>();

        public float Duration => duration;
        public AnimatorTriggerSymbol ThrowTrigger => throwTrigger;
        public IKBinding[] IKBindings => ikBindings;

        [Serializable]
        public class IKBinding
        {
            [Tooltip("IK goal on the VICTIM to activate.")]
            public AvatarIKGoal VictimGoal;

            [Tooltip("Anchor on the ATTACKER whose transform is used as the IK target.")]
            public AttachmentAnchorSymbol AttackerAnchor;
        }
    }
}
