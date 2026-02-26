using System;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Application.Characters
{
    /// <summary>
    /// Use case for spawning a character in the game world.
    /// Handles creation, registration, and lifecycle management.
    /// </summary>
    public class SpawnCharacterUseCase
    {
        readonly ICharacterFactoryRegistry factoryRegistry;
        readonly ICharacterRepository repository;
        readonly DisposableBag subscriptions = new DisposableBag();

        public SpawnCharacterUseCase(
            ICharacterFactoryRegistry factoryRegistry,
            ICharacterRepository repository)
        {
            this.factoryRegistry = factoryRegistry ?? throw new ArgumentNullException(nameof(factoryRegistry));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Execute the spawn character use case.
        /// </summary>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">World rotation to spawn with.</param>
        /// <param name="parameters">Parameters for character creation.</param>
        /// <returns>The spawned character instance.</returns>
        public ICharacter Execute(Vector3 position, Quaternion rotation, ICharacterCreationParameters parameters)
        {
            var factory = factoryRegistry.Get(parameters.GetType());
            var character = factory.Create(position, rotation, parameters);

            repository.Register(character);

            character.Destroyed.Subscribe(OnCharacterDestroyed).AddTo(subscriptions);

            return character;
        }

        void OnCharacterDestroyed(ICharacter character)
        {
            repository.Unregister(character.Id);
        }
    }
}
