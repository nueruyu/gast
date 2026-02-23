using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using R3.Triggers;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

namespace Cryst.Infrastructure.Characters
{
    public class Character : ICharacter
    {
        GameObject gameObject;
        readonly CancellationToken destroyCancellationToken;

        public Character(GameObject gameObject)
        {
            this.gameObject = gameObject;
            destroyCancellationToken = gameObject.GetCancellationTokenOnDestroy();
            destroyCancellationToken.Register(OnDestroy);
        }

        public void Initialize(
            CharacterContext context)
        {
            this.context = context;
        }

        public void RegisterFacets(
            Dictionary<Type, ICharacterFacet> facets)
        {
            this.facets = facets;
        }

        Dictionary<Type, ICharacterFacet> facets;
        CharacterContext context;

        readonly Signal<ICharacter> destroyedSignal = new();

        public CharacterId Id => context.CharacaterId;
        public ISignal<ICharacter> Destroyed => destroyedSignal;
        public CancellationToken CancellationToken => destroyCancellationToken;

        public bool Is<T>(out T facet) where T : class, ICharacterFacet
        {
            if (facets.TryGetValue(typeof(T), out var untypedFacet))
            {
                facet = (T)untypedFacet;
                return true;
            }

            facet = null;
            return false;
        }

        public T Resolve<T>() where T : class => context.Resolve<T>();

        public bool TryResolve<T>(out T module) where T : class => context.TryResolve(out module);

        public void Destroy()
        {
            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
                gameObject = null;
            }
        }

        void OnDestroy()
        {
            destroyedSignal.Publish(this);
        }
    }
}