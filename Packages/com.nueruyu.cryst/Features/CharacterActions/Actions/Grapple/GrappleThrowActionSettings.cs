using Gast.Unity.Features.Characters.IK;
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

        [Header("IK")]
        [Tooltip("Which IK goal on the VICTIM is controlled during the throw.")]
        [SerializeField]
        AvatarIKGoal victimIKGoal = AvatarIKGoal.LeftHand;

        [Tooltip("Anchor on the ATTACKER whose transform is used as the IK target for the victim's hand.")]
        [SerializeField]
        AttachmentAnchorSymbol attackerHandAnchor;

        public float Duration => duration;
        public AnimatorTriggerSymbol ThrowTrigger => throwTrigger;
        public AvatarIKGoal VictimIKGoal => victimIKGoal;
        public AttachmentAnchorSymbol AttackerHandAnchor => attackerHandAnchor;
    }
}
