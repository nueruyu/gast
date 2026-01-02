using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.UseCases.Characters
{
    /// <summary>
    /// Use case for creating an AI-controlled (NPC) character.
    /// It spawns a character and attaches an appropriate brain from the factory.
    /// </summary>
    public class CreateNpcUseCase
    {
        readonly SpawnCharacterUseCase spawnCharacterUseCase;
        readonly ICharacterBrainFactory characterBrainFactory;

        public CreateNpcUseCase(SpawnCharacterUseCase spawnCharacterUseCase, ICharacterBrainFactory characterBrainFactory)
        {
            this.spawnCharacterUseCase = spawnCharacterUseCase;
            this.characterBrainFactory = characterBrainFactory;
        }

        /// <summary>
        /// Executes the NPC character creation process.
        /// </summary>
        /// <param name="typeId">The character type identifier.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">World rotation to spawn with.</param>
        /// <param name="faction">The faction of the NPC.</param>
        /// <returns>The created and AI-controlled character instance.</returns>
        public ICharacter Execute(CharacterTypeId typeId, Vector3 position, Quaternion rotation, Faction faction)
        {
            var character = spawnCharacterUseCase.Execute(typeId, position, rotation, faction);
            var brain = characterBrainFactory.Create(typeId);
            character.AttachBrain(brain);
            return character;
        }
    }
}