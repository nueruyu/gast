using System;

namespace Gast.Domain.Characters
{
    public interface IAspectFactoryRegistry
    {
        IAspectFactory Get(Type aspectType);
    }
}