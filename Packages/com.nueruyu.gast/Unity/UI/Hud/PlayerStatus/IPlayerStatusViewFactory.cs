using System.Threading;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Hud.PlayerStatus
{
    public interface IPlayerStatusViewFactory
    {
        VisualElement Create(CancellationToken cancellationToken);
    }
}
