using Gast.Domain.AI;
using Gast.Domain.Characters;
using GastGame.AI;
using Gast.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using VContainer;

namespace GastGame.Factories
{
    public class CharacterAIBrainFactory : ICharacterAIBrainFactory
    {
        readonly IObjectResolver resolver;

        public CharacterAIBrainFactory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public ICharacterAIBrain Create()
        {
            return resolver.Resolve<AIBrain>();
        }
    }
}