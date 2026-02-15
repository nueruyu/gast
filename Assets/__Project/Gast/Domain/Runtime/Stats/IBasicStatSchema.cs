namespace Gast.Domain.Stats
{
    public interface IBasicStatSchema : IStatSchema
    {
        IStatDefinition<float> Health { get; }
        IStatDefinition<float> MaxHealth { get; }
    }
}