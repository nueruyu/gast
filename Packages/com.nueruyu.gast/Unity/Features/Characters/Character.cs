using System;
using System.Collections.Generic;
using System.Threading;
using Gast.Core.Observables;
using Gast.Domain.Characters;

namespace Gast.Unity.Features.Characters
{
    public class Character : ICharacter
    {
        readonly CharacterContext context;
        readonly Dictionary<Type, ICharacterFacet> facets;
        readonly Signal<ICharacter> destroyedSignal = new();
        bool isDestroyed;

        public Character(
            CharacterContext context,
            Dictionary<Type, ICharacterFacet> facets)
        {
            this.context = context;
            this.facets = facets;
            context.CancellationToken.Register(OnDestroy);
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

        public void Destroy()
        {
            if (isDestroyed)
                return;

            UnityEngine.Object.Destroy(context.GameObject);
        }

        void OnDestroy()
        {
            isDestroyed = true;
            destroyedSignal.Publish(this);
        }
    }
}