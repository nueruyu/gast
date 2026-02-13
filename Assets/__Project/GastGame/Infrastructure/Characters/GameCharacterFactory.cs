using System;
using Gast.Domain.Characters;
using GastGame.Domain.Characters;
using GastGame.Features.Characters;

namespace GastGame.Infrastructure.Characters
{
    public class GameCharacterFactory : ICharacterAspectFactory
    {
        public Type AspectType => typeof(IGameCharacter);

        public ICharacterAspect Create(ICharacter character)
        {
            var stateStore = character.Resolve<CharacterActionStateStore>();
            return new GameCharacter(character, stateStore);
        }
    }
}
