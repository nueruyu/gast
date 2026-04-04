using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.Unity.Shared.Attachments
{
    public class AttachmentAnchorRegistry
    {
        readonly Dictionary<AttachmentAnchorSymbol, Transform> anchors;

        public AttachmentAnchorRegistry(IEnumerable<AttachmentAnchor> anchorComponents)
        {
            anchors = anchorComponents
                .Where(a => a != null && a.Symbol != null)
                .ToDictionary(a => a.Symbol, a => a.transform);
        }

        public bool TryGetAnchor(AttachmentAnchorSymbol symbol, out Transform anchorTransform)
        {
            if (symbol == null)
            {
                anchorTransform = null;
                return false;
            }
            return anchors.TryGetValue(symbol, out anchorTransform);
        }
    }
}
