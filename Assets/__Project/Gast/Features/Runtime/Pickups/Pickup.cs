using Cysharp.Threading.Tasks;
using Gast.Api.Economy;
using Gast.Core.Commands;
using Gast.Core.Observables;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Gast.Features.Characters;
using Gast.Features.Interactions;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Features.Pickups
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Interactable))]
    public class Pickup : MonoBehaviour, IPickup
    {
        [Tooltip("Force applied to spawned loot")]
        [SerializeField]
        float dropImpulse = 2f;

        ICommandDispatcher commandDispatcher;
        PickupId id;
        ItemId itemId;
        int quantity;
        Rigidbody rb;
        Interactable interactable;
        readonly Signal<IPickup> destroyedSignal = new();

        public PickupId Id => id;
        public int Quantity => quantity;
        public Vector3 Position => transform.position;
        public ISignal<IPickup> Destroyed => destroyedSignal;

        public void Initialize(PickupId id, ItemId itemId, string itemName, int quantity, ICommandDispatcher commandDispatcher)
        {
            this.id = id;
            this.itemId = itemId;
            this.quantity = quantity;
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));

            TryGetComponent(out rb);
            TryGetComponent(out interactable);

            interactable.Config.Prompt = $"{itemName} x{quantity}";

            interactable.Interacted
                .Subscribe(OnInteract)
                .AddTo(destroyCancellationToken);
        }

        public void Eject()
        {
            var force = (Vector3.up + Random.insideUnitSphere * 0.2f).normalized * dropImpulse;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(0.1f * dropImpulse * Random.insideUnitSphere, ForceMode.Impulse);
        }

        void OnInteract(Character interactor)
        {
            if (commandDispatcher == null)
                throw new InvalidOperationException(
                    $"Pickup on {gameObject.name} not properly initialized with ICommandDispatcher");

            if (commandDispatcher.Dispatch<PickUpItemCommand, bool>(
                new(interactor.Id, itemId, quantity)))
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            destroyedSignal.Publish(this);
        }
    }
}