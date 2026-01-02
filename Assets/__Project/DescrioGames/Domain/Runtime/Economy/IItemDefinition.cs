namespace DescrioGames.Domain.Economy
{
    /// <summary>
    /// Item definition data.
    /// </summary>
    public interface IItemDefinition
    {
        public ItemId Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaxStack { get; }
        public string Description { get; }
    }
}