using Gast.Core.DI;
using VContainer;

namespace Gast.Composition
{
    public class VContainerRegistrationBuilder : IRegistrationBuilder
    {
        readonly RegistrationBuilder registrationBuilder;

        public VContainerRegistrationBuilder(RegistrationBuilder registrationBuilder)
        {
            this.registrationBuilder = registrationBuilder;
        }

        public IRegistrationBuilder As<TInterface>()
        {
            registrationBuilder.As<TInterface>();
            return this;
        }

        public IRegistrationBuilder AsImplementedInterfaces()
        {
            registrationBuilder.AsImplementedInterfaces();
            return this;
        }

        public IRegistrationBuilder AsSelf()
        {
            registrationBuilder.AsSelf();
            return this;
        }
    }
}