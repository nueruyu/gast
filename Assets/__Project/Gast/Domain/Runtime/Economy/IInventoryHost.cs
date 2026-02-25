using Gast.Domain.Characters;

namespace Gast.Domain.Economy
{
    public interface IInventoryHost : ICharacterFacet
    {
        Inventory Inventory { get; }
    }
}