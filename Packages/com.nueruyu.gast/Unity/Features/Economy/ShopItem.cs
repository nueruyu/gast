using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Unity.Features.Interactions;
using UnityEngine;

namespace Gast.Unity.Features.Economy
{
    [RequireComponent(typeof(Interactable))]
    public class ShopItem : MonoBehaviour
    {
        [SerializeField]
        ItemReference itemReference;

        Interactable interactable;

        readonly Signal<CharacterId> buy = new();

        public ItemId ItemId => itemReference.Id;
        public ISignal<CharacterId> Buy => buy;

        void Awake()
        {
            TryGetComponent(out interactable);

            interactable.Interacted
                .Subscribe(OnInteract)
                .AddTo(destroyCancellationToken);
        }

        public void Initialize(InteractionSystem interactionSystem, string interactionPrompt)
        {
            interactable.Initialize(interactionSystem);
            interactable.Config.Prompt = interactionPrompt;
        }

        void OnInteract(ICharacter interactor)
        {
            buy.Publish(interactor.Id);
        }
    }
}