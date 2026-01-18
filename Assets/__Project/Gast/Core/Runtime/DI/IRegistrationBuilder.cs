namespace Gast.Core.DI
{
    public interface IRegistrationBuilder
    {
        IRegistrationBuilder As<TInterface>();
        IRegistrationBuilder AsImplementedInterfaces();
        IRegistrationBuilder AsSelf();
    }
}
