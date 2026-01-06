using System;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.UseCases.Characters
{
    /// <summary>
    /// Use case for spawning a character in the game world.
    /// Handles creation, registration, and lifecycle management.
    /// </summary>
    public class SpawnCharacterUseCase
    {
        readonly ICharacterFactory factory;
        readonly ICharacterRepository repository;
        readonly DisposableBag subscriptions = new DisposableBag();

        public SpawnCharacterUseCase(
            ICharacterFactory factory,
            ICharacterRepository repository)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Execute the spawn character use case.
        /// </summary>
        /// <param name="typeId">Character type identifier.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">World rotation to spawn with.</param>
        /// <param name="faction">Optional faction override.</param>
        /// <returns>The spawned character instance.</returns>
        public ICharacter Execute(CharacterTypeId typeId, Vector3 position, Quaternion rotation, Faction faction)
        {
            var character = factory.Create(typeId, position, rotation, faction);

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