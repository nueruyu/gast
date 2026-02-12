namespace Gast.Domain.Stats
{
    public interface IStatRegistrar
    {
        void Register<T>(IStatDefinition<T> definition, T initialValue);
    }
}
