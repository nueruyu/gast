using System;

namespace Gast.Domain.Characters
{
    public interface ICharacterAspectFactoryRegistry
    {
        ICharacterAspectFactory Get(Type aspectType);
    }
}