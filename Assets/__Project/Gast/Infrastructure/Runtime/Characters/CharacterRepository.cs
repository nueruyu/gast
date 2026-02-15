using Gast.Core.Observables;
using Gast.Domain.Characters;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Infrastructure.Characters
{
    /// <summary>
    /// In-memory repository for managing runtime character instances.
    /// </summary>
    public class CharacterRepository : ICharacterRepository
    {
        readonly Dictionary<CharacterId, ICharacter> characters = new();
        readonly Signal<ICharacter> registeredSignal = new();
        readonly Signal<ICharacter> unregisteredSignal = new();

        public ISignal<ICharacter> Registered => registeredSignal;
        public ISignal<ICharacter> Unregistered => unregisteredSignal;

        public void Register(ICharacter character)
        {
            if (character == null)
                throw new System.ArgumentNullException(nameof(character));

            characters[character.Id] = character;
            registeredSignal.Publish(character);
        }

        public void Unregister(CharacterId id)
        {
            if (characters.Remove(id, out var character))
            {
                unregisteredSignal.Publish(character);
            }
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
    }
}