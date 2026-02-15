using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Composition root that connects a brain (decision-making) with a body (physics execution).
    /// </summary>
    [RequireComponent(typeof(CharacterBody))]
    public class Character : MonoBehaviour, ICharacter
    {
        ICharacterTypeDefinition typeDefinition;
        ICharacterBrain currentBrain;
        ICharacterAspectFactoryRegistry aspectFactoryRegistry;
        readonly Dictionary<Type, ICharacterAspect> aspectCache = new();

        CharacterActionController actionController;
        CharacterContext context;

        readonly Signal<ICharacter> destroyedSignal = new();

        public CharacterId Id => context.Id;
        public CharacterTypeId TypeId => context.TypeId;
        public ICharacterStatus Status { get; private set; }
        public Faction Faction { get; private set; }
        public Wallet Wallet { get; private set; }
        public Inventory Inventory { get; private set; }
        public ICharacterTypeDefinition TypeDefinition => typeDefinition;
        public IVisionSensor VisionSensor => context.VisionSensor;
        public IInteractionSensor InteractionSensor => context.InteractionSensor;
        public INavigationProvider NavigationProvider => context.NavigationProvider;
        public ICharacterBody Body => context.Body;
        public ICharacterActionController ActionController => actionController;
        public ISignal<ICharacter> Destroyed => destroyedSignal;
        public CancellationToken CancellationToken => destroyCancellationToken;

        void Update()
        {
            actionController?.Update();
        }

        /// <summary>
        /// Initialize the character with a specific type and faction.
        /// Called by CharacterFactory after instantiation.
        /// </summary>
        public void Initialize(
            CharacterContext context,
            ICharacterTypeDefinition typeDefinition,
            CharacterActionController actionController,
            ICharacterStatus status,
            Faction faction,
            Wallet wallet,
            Inventory inventory,
            ICharacterAspectFactoryRegistry aspectFactoryRegistry)
        {
            this.context = context;
            this.typeDefinition = typeDefinition;
            this.actionController = actionController;
            Status = status;
            Faction = faction;
            Wallet = wallet;
            Inventory = inventory;
            this.aspectFactoryRegistry = aspectFactoryRegistry;
        }

        public T As<T>() where T : class, ICharacterAspect
        {
            if (aspectCache.TryGetValue(typeof(T), out var aspect))
            {
                return (T)aspect;
            }

            var factory = aspectFactoryRegistry.Get(typeof(T)) ??
                throw new InvalidOperationException($"No aspect factory registered for type {typeof(T)}");

            var newAspect = (T)factory.Create(this);
            aspectCache[typeof(T)] = newAspect;
            return newAspect;
        }

        public T Resolve<T>() where T : class => context.Resolve<T>();

        /// <summary>
        /// Attach a brain to this character, detaching any existing brain first.
        /// The brain's OnAttached method will be called after attachment.
        /// </summary>
        public void AttachBrain(ICharacterBrain newBrain)
        {
            currentBrain?.OnDetached();

            currentBrain = newBrain;
            currentBrain?.OnAttached(this);
        }

        /// <summary>
        /// Detach the current brain from this character.
        /// The brain's OnDetached method will be called.
        /// </summary>
        public void DetachBrain()
        {
            currentBrain?.OnDetached();
            currentBrain = null;
        }

        public void Destroy()
        {
            if (this != null && gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            DetachBrain();
            destroyedSignal.Publish(this);
        }
    }
}