using UnityEngine;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Features.Interactions;
using Cysharp.Threading.Tasks;
using Gast.Features.Characters;
using System;
using Gast.Core.Observables;

namespace Gast.Features.Economy
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

        public void SetInteractionPrompt(string prompt)
        {
            interactable.Config.Prompt = prompt;
        }

        void OnInteract(Character interactor)
        {
            buy.Publish(interactor.Id);
        }
    }
}