using Gast.Domain.Characters;

namespace Gast.Domain.Combat
{
    public interface IEffect
    {
        bool CanApplyTo(ICharacter target);
    }
}