using UnityEngine;

namespace Gast.Unity.Shared.Attachments
{
    public class AttachmentAnchor : MonoBehaviour
    {
        [SerializeField]
        AttachmentAnchorSymbol symbol;

        public AttachmentAnchorSymbol Symbol => symbol;
    }
}
