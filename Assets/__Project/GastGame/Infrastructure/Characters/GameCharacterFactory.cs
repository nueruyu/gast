using System;
using Gast.Domain.Characters;
using GastGame.Domain.Characters;

namespace GastGame.Infrastructure.Characters
{
    public class GameCharacterFactory : ICharacterAspectFactory
    {
        public Type AspectType => typeof(IGameCharacter);

        public ICharacterAspect Create(ICharacter character)
        {
            return new GameCharacter(character);
        }
    }
}