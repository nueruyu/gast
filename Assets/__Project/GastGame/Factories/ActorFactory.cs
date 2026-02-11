using System;
using Gast.Domain.Characters;
using GastGame.Actors;

namespace GastGame.Factories
{
    public class ActorFactory : IAspectFactory
    {
        public Type AspectType => typeof(IActor);

        public ICharacterAspect Create(ICharacter character)
        {
            return new Actor(character);
        }
    }
}
