namespace Gast.Core.DI
{
    public interface IContainerBuilder
    {
        IRegistrationBuilder Register<T>(Lifetime lifetime = Lifetime.Singleton);
        void RegisterInstance<T>(T instance) where T : class;
    }
}
