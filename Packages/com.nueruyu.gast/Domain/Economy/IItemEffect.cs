using Gast.Domain.Characters;

namespace Gast.Domain.Economy
{
    public interface IItemEffect
    {
        void Apply(ICharacter character);
    }
}
