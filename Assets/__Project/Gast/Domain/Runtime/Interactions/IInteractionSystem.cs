using Gast.Core.Observables;
using Gast.Domain.Characters;
using System.Threading.Tasks;

namespace Gast.Domain.Interactions
{
    public interface IInteractionSystem
    {
        ILive<IInteractable> CurrentInteractable { get; }
        ISignal<IInteractable> InteractionCompleted { get; }
        ILive<float> InteractionProgress { get; }

        bool TryInteract();

        void CancelInteraction();

        void SetInteractor(CharacterId interactorId);

        void UnsetInteractor();

        ValueTask<bool> RequestInteractionAsync(CharacterId interactorId, InteractableId interactableId);
    }
}