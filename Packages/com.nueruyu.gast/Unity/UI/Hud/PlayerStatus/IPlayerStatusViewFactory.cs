using System.Threading;
using UnityEngine.UIElements;

namespace Gast.UI.Hud.Status
{
    public interface IPlayerStatusViewFactory
    {
        VisualElement Create(CancellationToken cancellationToken);
    }
}
