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
    public class Character : MonoBehaviour, ICharacter
    {
        ICharacterTypeDefinition typeDefinition;
        ICharacterBrain currentBrain;
        ICharacterFacetFactoryRegistry facetFactoryRegistry;
        readonly Dictionary<Type, ICharacterFacet> facetCache = new();

        CharacterActionController actionController;
        CharacterContext context;

        readonly Signal<ICharacter> destroyedSignal = new();

        public CharacterId Id => context.CharacaterId;
        public CharacterTypeId TypeId => context.TypeId;
        public Faction Faction { get; private set; }
        public Wallet Wallet { get; private set; }
        public Inventory Inventory { get; private set; }
        public ICharacterTypeDefinition TypeDefinition => typeDefinition;
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
            Faction faction,
            Wallet wallet,
            Inventory inventory,
            ICharacterFacetFactoryRegistry facetFactoryRegistry)
        {
            this.context = context;
            this.typeDefinition = typeDefinition;
            this.actionController = actionController;
            Faction = faction;
            Wallet = wallet;
            Inventory = inventory;
            this.facetFactoryRegistry = facetFactoryRegistry;
        }

        public T As<T>() where T : class, ICharacterFacet
        {
            if (facetCache.TryGetValue(typeof(T), out var facet))
            {
                return (T)facet;
            }

            var factory = facetFactoryRegistry.Get(typeof(T)) ??
                throw new InvalidOperationException($"No facet factory registered for type {typeof(T)}");

            var newFacet = (T)factory.Create(this);
            facetCache[typeof(T)] = newFacet;
            return newFacet;
        }

        public T Resolve<T>() where T : class => context.Resolve<T>();

        public bool TryResolve<T>(out T module) where T : class => context.TryResolve(out module);

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