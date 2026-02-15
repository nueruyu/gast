using System;

namespace Gast.Domain.Characters
{
    public interface ICharacterAspectFactory
    {
        Type AspectType { get; }
        ICharacterAspect Create(ICharacter character);
    }
}
