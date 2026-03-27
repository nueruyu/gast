using Gast.Application.AI;
using Gast.Application.AIPlanning;
using Gast.Application.Economy;
using Gast.Core.DI;
using Gast.Domain.Characters;
using Gast.Lib.AI.Debugging;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.AI;
using Gast.Unity.Infrastructure.Characters;
using Gast.Unity.Infrastructure.HitDetection;
using Gast.Unity.Infrastructure.Items;
using Gast.Unity.Infrastructure.Pickups;
using Gast.Unity.Infrastructure.Remoting.AI;
using Gast.Unity.Infrastructure.Services;
using Gast.Unity.Infrastructure.Stories;
using Gast.Unity.Infrastructure.Stories.Factories;

namespace Gast.Unity.Infrastructure
{
    public class InfrastructureInstaller : IInstaller
    {
        readonly MockAIPlanningSettings mockAIPlanningSettings;
        readonly MockStoryGenerationSettings mockStoryGenerationSettings;

        public InfrastructureInstaller(
            MockAIPlanningSettings mockAIPlanningSettings,
            MockStoryGenerationSettings mockStoryGenerationSettings)
        {
            this.mockAIPlanningSettings = mockAIPlanningSettings;
            this.mockStoryGenerationSettings = mockStoryGenerationSettings;
        }

        public void Install(IContainerBuilder builder)
        {
            // Command & Event System
            builder.Register<CommandDispatcher>().AsImplementedInterfaces();
            builder.Register<JsonCommandSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DomainEventPublisher>().AsImplementedInterfaces();

            // Register GaiaPlanningClient only when at least one service needs the real API
            bool needsRealGaiaClient = !mockAIPlanningSettings.IsEnabled || !mockStoryGenerationSettings.IsEnabled;
            if (needsRealGaiaClient)
            {
                builder.Register<GaiaPlanningClient>().As<IGaiaPlanningClient>();
            }

            // AI Planning
            if (mockAIPlanningSettings.IsEnabled)
            {
                builder.Register<MockAIPlanningService>().As<IAIPlanningService>();
            }
            else
            {
                builder.Register<AIPlanningService>().As<IAIPlanningService>();
            }

            // Story Generation
            if (mockStoryGenerationSettings.IsEnabled)
            {
                builder.Register<MockStoryGenerationService>().As<IStoryGenerationService>();
            }
            else
            {
                builder.Register<StoryGenerationService>().As<IStoryGenerationService>();
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

            // Story
            builder.Register<StoryActorContext>(Lifetime.Singleton);
            builder.Register<DynamicStoryDomainFactory>(Lifetime.Singleton);
            builder.Register<StorySystem>(Lifetime.Singleton).As<IStoryRunner>();
            builder.Register<SetObjectivesActionFactory>(Lifetime.Singleton).As<IStoryActionFactory>();
            builder.Register<WaitForCharacterDefeatedActionFactory>(Lifetime.Singleton).As<IStoryActionFactory>();
            builder.Register<WaitForItemAcquiredActionFactory>(Lifetime.Singleton).As<IStoryActionFactory>();
        }
    }
}
