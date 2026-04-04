using Gast.Domain.Characters;
using Gast.Unity.Features.Characters.IK;
using Gast.Unity.Shared.Attachments;

namespace Cryst.Features.Characters
{
    /// <summary>
    /// Exposes a character's rig components (IK controller and attachment anchors) as a facet,
    /// allowing other characters' actions to read anchor positions or drive IK goals on this character.
    /// </summary>
    public class CharacterRigFacet : ICharacterFacet
    {
        public AttachmentAnchorRegistry AnchorRegistry { get; }
        public CharacterIKController IKController { get; }

        public CharacterRigFacet(AttachmentAnchorRegistry anchorRegistry, CharacterIKController ikController)
        {
            AnchorRegistry = anchorRegistry;
            IKController = ikController;
        }
    }
}
