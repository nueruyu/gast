using System;
using Gast.Domain.Characters;
using GastGame.Domain.Characters;

namespace GastGame.Infrastructure.Characters
{
    public class GameCharacterFactory : IAspectFactory
    {
        public Type AspectType => typeof(IGameCharacter);

        public ICharacterAspect Create(ICharacter character)
        {
            return new GameCharacter(character);
        }
    }
}