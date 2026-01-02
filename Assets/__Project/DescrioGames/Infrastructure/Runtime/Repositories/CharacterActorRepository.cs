using DescrioGames.Domain.Characters;
using DescrioGames.Features.Characters;
using System.Collections.Generic;

namespace DescrioGames.Infrastructure.Repositories
{
    public class CharacterActorRepository : ICharacterActorRepository
    {
        readonly Dictionary<CharacterId, Character> characters = new();

        public void Register(Character character)
        {
            if (character == null)
                throw new System.ArgumentNullException(nameof(character));

            characters[character.Id] = character;
        }

        public void Unregister(CharacterId id)
        {
            characters.Remove(id);
        }

        public Character Get(CharacterId id)
        {
            if (characters.TryGetValue(id, out var character))
                return character;

            throw new KeyNotFoundException($"Character with ID {id} not found in repository.");
        }
    }
}