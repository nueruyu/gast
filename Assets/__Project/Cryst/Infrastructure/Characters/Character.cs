using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using R3;
using R3.Triggers;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class Character : ICharacter
    {
        readonly CharacterContext context;
        readonly Dictionary<Type, ICharacterFacet> facets;
        readonly Signal<ICharacter> destroyedSignal = new();
        GameObject gameObject;

        public Character(
            CharacterContext context,
            Dictionary<Type, ICharacterFacet> facets)
        {
            this.context = context;
            this.facets = facets;
            gameObject = context.GameObject;
            gameObject.OnDestroyAsObservable().Subscribe(_ => OnDestroy());
        }

        public CharacterId Id => context.CharacaterId;
        public ISignal<ICharacter> Destroyed => destroyedSignal;
        public CancellationToken CancellationToken => context.CancellationToken;

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