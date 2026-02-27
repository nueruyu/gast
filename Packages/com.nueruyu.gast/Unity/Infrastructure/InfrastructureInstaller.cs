using Gast.Application.AI;
using Gast.Application.AIPlanning;
using Gast.Application.Economy;
using Gast.Core.DI;
using Gast.Domain.Characters;
using Gast.Lib.AI.Debugging;
using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.AI;
using Gast.Unity.Infrastructure.Characters;
using Gast.Unity.Infrastructure.HitDetection;
using Gast.Unity.Infrastructure.Items;
using Gast.Unity.Infrastructure.Pickups;
using Gast.Unity.Infrastructure.Remoting.AI;
using Gast.Unity.Infrastructure.Services;

namespace Gast.Unity.Infrastructure
{
    public class InfrastructureInstaller : IInstaller
    {
        readonly MockAIPlanningSettings mockAIPlanningSettings;

        public InfrastructureInstaller(MockAIPlanningSettings mockAIPlanningSettings)
        {
            this.mockAIPlanningSettings = mockAIPlanningSettings;
        }

        public void Install(IContainerBuilder builder)
        {
            // Command & Event System
            builder.Register<CommandDispatcher>().AsImplementedInterfaces();
            builder.Register<JsonCommandSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DomainEventPublisher>().AsImplementedInterfaces();

            // AI Server Client / Mock
            if (mockAIPlanningSettings.IsEnabled)
            {
                builder.Register<MockAIPlanningService>().As<IAIPlanningService>();
            }
            else
            {
                builder.Register<GaiaPlanningClient>().As<IGaiaPlanningClient>();
                builder.Register<AIPlanningService>().As<IAIPlanningService>();
            }

            // Character
            builder.Register<CharacterFactoryRegistry>().As<ICharacterFactoryRegistry>();
            builder.Register<CharacterRepository>().AsImplementedInterfaces();
            builder.Register<CharacterTypeRepository>().AsImplementedInterfaces().AsSelf();
            builder.Register<CharacterBrainManager>().AsImplementedInterfaces();

            // Combat
            builder.Register<HitAreaFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            // Economy
            builder.Register<ItemRepository>().AsImplementedInterfaces().AsSelf();
            builder.Register<ItemAssetService>().As<IItemAssetService>();
            builder.Register<ShopInitializer>().AsImplementedInterfaces();

            // Pickups
            builder.Register<PickupFactory>().AsImplementedInterfaces();
            builder.Register<PickupRepository>().AsImplementedInterfaces();

            // AI
            builder.Register<AIDebugger>().AsSelf().As<IContextRegistry>().As<IAIDebugger>();
            builder.Register<AIDebugInitializer>().AsImplementedInterfaces();

            // AI Tools & Objectives
            builder.Register<ReflectionToolRegistry>(Lifetime.Singleton).As<IToolRegistry>();
            builder.Register<ReflectionObjectiveRegistry>(Lifetime.Singleton).As<IObjectiveRegistry>();
            builder.Register<GoalInstantiator>(Lifetime.Singleton);
            builder.Register<PlanConverter>(Lifetime.Singleton);
        }
    }
}