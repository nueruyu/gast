using System;

namespace Gast.Domain.Characters
{
    public interface IAspectFactory
    {
        Type AspectType { get; }
        ICharacterAspect Create(ICharacter character);
    }
}
