using DescrioGames.Core.Observables;
using DescrioGames.Domain.Characters;

namespace DescrioGames.Domain.Interactions
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
    }
}