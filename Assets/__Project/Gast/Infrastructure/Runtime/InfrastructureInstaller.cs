using Gast.Application.AI;
using Gast.Application.AIPlanning;
using Gast.Application.Economy;
using Gast.Core.DI;
using Gast.Features.Combat;
using Gast.Infrastructure.AI;
using Gast.Infrastructure.Characters;
using Gast.Infrastructure.Items;
using Gast.Infrastructure.Pickups;
using Gast.Infrastructure.Remoting.AI;
using Gast.Infrastructure.Services;
using Gast.Lib.AI.Debugging;
using Gast.Infrastructure.Combat;
using Gast.Lib.Gaia;

namespace Gast.Infrastructure
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
            builder.Register<JsonCommandSerializer>().AsImplementedInterfaces();
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

            // Facets
            builder.Register<CharacterFacetFactoryRegistry>().AsImplementedInterfaces();

            // Character
            builder.Register<CharacterFactory>().AsImplementedInterfaces();
            builder.Register<CharacterRepository>().AsImplementedInterfaces();
            builder.Register<CharacterTypeRepository>().AsImplementedInterfaces().AsSelf();

            // Combat
            builder.Register<CombatFeedbackService>();
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
            builder.Register<ReflectionToolRegistry>().As<IToolRegistry>();
            builder.Register<ReflectionObjectiveRegistry>().As<IObjectiveRegistry>();
            builder.Register<GoalInstantiator>();
            builder.Register<PlanConverter>();
        }
    }
}