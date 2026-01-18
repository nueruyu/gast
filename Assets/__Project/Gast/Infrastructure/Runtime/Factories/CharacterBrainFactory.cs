using Gast.Domain.Characters;
using Gast.Features.AI;
using Gast.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using VContainer;

namespace Gast.Infrastructure.Factories
{
    public class CharacterBrainFactory : ICharacterBrainFactory
    {
        readonly IObjectResolver resolver;

        public CharacterBrainFactory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public ICharacterBrain Create(CharacterTypeId typeId)
        {
            return CreateBrain();
        }

        ICharacterBrain CreateBrain()
        {
            return resolver.Resolve<AIBrain>();
        }
    }
}