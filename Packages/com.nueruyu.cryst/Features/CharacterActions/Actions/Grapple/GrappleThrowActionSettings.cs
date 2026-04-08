using System;
using Gast.Unity.Features.Characters;
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

        [Header("Positioning")]
        [SerializeField]
        Vector3 grappledPositionOffset = new Vector3(0f, 0f, 0.6f);

        [SerializeField]
        Vector3 grappledRotationOffset = Vector3.zero;

        [Header("IK")]
        [Tooltip("IK goals to activate on the VICTIM, each mapped to an anchor on the ATTACKER.")]
        [SerializeField]
        IKBinding[] ikBindings = Array.Empty<IKBinding>();

        public float Duration => duration;
        public AnimatorTriggerSymbol ThrowTrigger => throwTrigger;
        public Vector3 GrappledPositionOffset => grappledPositionOffset;
        public Vector3 GrappledRotationOffset => grappledRotationOffset;
        public IKBinding[] IKBindings => ikBindings;

        [Serializable]
        public class IKBinding
        {
            [Tooltip("IK goal (as anchor symbol) on the VICTIM to activate.")]
            public AttachmentAnchorSymbol VictimGoal;

            [Tooltip("Anchor on the ATTACKER whose transform is used as the IK target.")]
            public AttachmentAnchorSymbol AttackerAnchor;
        }
    }
}
