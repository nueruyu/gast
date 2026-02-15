using System;
using Gast.Domain.Characters;
using Cryst.Domain.Characters;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacterFactory : ICharacterAspectFactory
    {
        public Type AspectType => typeof(ICrystCharacter);

        public ICharacterAspect Create(ICharacter character)
        {
            return new CrystCharacter(character);
        }
    }
}