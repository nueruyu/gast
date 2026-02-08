using Gast.Core.Observables;
using Gast.Domain.Characters;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory repository for managing runtime character instances.
    /// </summary>
    public class CharacterRepository : ICharacterRepository
    {
        readonly Dictionary<CharacterId, ICharacter> characters = new();
        readonly Signal<ICharacter> registeredSignal = new();

        public ISignal<ICharacter> Registered => registeredSignal;

        public void Register(ICharacter character)
        {
            if (character == null)
                throw new System.ArgumentNullException(nameof(character));

            characters[character.Id] = character;
            registeredSignal.Publish(character);
        }

        public void Unregister(CharacterId id)
        {
            characters.Remove(id);
        }

        public ICharacter Get(CharacterId id)
        {
            if (characters.TryGetValue(id, out var character))
                return character;

            throw new KeyNotFoundException($"Character with ID {id} not found in repository.");
        }

        public IEnumerable<ICharacter> GetAll()
        {
            return characters.Values;
        }

        public IEnumerable<ICharacter> GetByFaction(Faction faction)
        {
            return characters.Values.Where(c => c.Faction == faction);
        }
    }
}