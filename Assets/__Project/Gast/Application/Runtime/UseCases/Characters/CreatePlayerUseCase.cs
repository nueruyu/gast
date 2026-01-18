using Gast.Domain.Characters;
using Gast.Domain.Players;
using UnityEngine;

namespace Gast.Application.UseCases.Characters
{
    /// <summary>
    /// Use case for creating a player-controlled character.
    /// It spawns a character and uses the PlayerManager to possess it.
    /// </summary>
    public class CreatePlayerUseCase
    {
        readonly SpawnCharacterUseCase spawnCharacterUseCase;
        readonly IPlayerManager playerManager;

        public CreatePlayerUseCase(SpawnCharacterUseCase spawnCharacterUseCase, IPlayerManager playerManager)
        {
            this.spawnCharacterUseCase = spawnCharacterUseCase;
            this.playerManager = playerManager;
        }

        /// <summary>
        /// Executes the player character creation and possession process.
        /// </summary>
        /// <param name="typeId">The character type identifier.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">World rotation to spawn with.</param>
        /// <returns>The created and player-possessed character instance.</returns>
        public ICharacter Execute(CharacterTypeId typeId, Vector3 position, Quaternion rotation)
        {
            var character = spawnCharacterUseCase.Execute(typeId, position, rotation, Faction.Player);
            playerManager.Possess(character.Id);
            return character;
        }
    }
}