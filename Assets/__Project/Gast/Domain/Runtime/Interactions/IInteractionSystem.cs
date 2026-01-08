using Gast.Domain.Characters;
using System.Threading;
using System.Threading.Tasks;

namespace Gast.Domain.Interactions
{
    public interface IInteractionSystem
    {
        ValueTask<bool> RequestInteractionAsync(
            CharacterId interactorId,
            InteractableId interactableId,
            CancellationToken cancellationToken = default);
    }
}