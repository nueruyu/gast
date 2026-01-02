namespace Gast.Domain.Economy
{
    public interface IItemRepository
    {
        IItemDefinition Get(ItemId id);
    }
}