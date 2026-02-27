using System;
using Gast.Core.DI;
using VContainer;
using IContainerBuilder = Gast.Core.DI.IContainerBuilder;
using Lifetime = Gast.Core.DI.Lifetime;

namespace Gast.Unity.Composition
{
    public class VContainerBuilder : IContainerBuilder
    {
        readonly VContainer.IContainerBuilder builder;

        public VContainerBuilder(VContainer.IContainerBuilder builder)
        {
            this.builder = builder;
        }

        public IRegistrationBuilder Register<T>(Lifetime lifetime)
        {
            var vContainerLifetime = ConvertLifetime(lifetime);
            var registrationBuilder = builder.Register<T>(vContainerLifetime);
            return new VContainerRegistrationBuilder(registrationBuilder);
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            builder.RegisterInstance(instance);
        }

        static VContainer.Lifetime ConvertLifetime(Lifetime lifetime)
        {
            return lifetime switch
            {
                Lifetime.Singleton => VContainer.Lifetime.Singleton,
                Lifetime.Scoped => VContainer.Lifetime.Scoped,
                Lifetime.Transient => VContainer.Lifetime.Transient,
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }
    }
}