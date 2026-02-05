using Gast.Application.AI;
using Gast.Application.AIPlanning;
using Gast.Application.Economy;
using Gast.Core.DI;
using Gast.Features.Combat;
using Gast.Infrastructure.AI;
using Gast.Infrastructure.Factories;
using Gast.Infrastructure.Remoting.AI;
using Gast.Infrastructure.Repositories;
using Gast.Infrastructure.Services;
using Gast.Infrastructure.Settings;
using Gast.Lib.AI.Debugging;
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

            // Character
            builder.Register<CharacterRepository>().AsImplementedInterfaces();
            builder.Register<CharacterActorRepository>().AsImplementedInterfaces();
            builder.Register<CharacterTypeRepository>().AsImplementedInterfaces().AsSelf();
            builder.Register<CharacterFactory>().AsImplementedInterfaces();
            builder.Register<CharacterAIBrainFactory>().AsImplementedInterfaces();
            builder.Register<CharacterFootstepService>().AsImplementedInterfaces().AsSelf();

            // Combat
            builder.Register<CombatFeedbackService>();
            builder.Register<CombatMethodFactory>().AsImplementedInterfaces();

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