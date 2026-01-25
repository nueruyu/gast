using Gast.Application.AI.Objectives;
using Gast.Application.AI.Tools;
using Gast.Core.DI;
using Gast.Features.Combat;
using Gast.Infrastructure.AI.Objectives;
using Gast.Infrastructure.AI.Tools;
using Gast.Infrastructure.Factories;
using Gast.Infrastructure.Remoting.AI;
using Gast.Infrastructure.Repositories;
using Gast.Infrastructure.Services;
using Gast.Lib.AI.Debugging;

namespace Gast.Infrastructure
{
    public class InfrastructureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Command & Event System
            builder.Register<CommandDispatcher>().AsImplementedInterfaces();
            builder.Register<JsonCommandSerializer>().AsImplementedInterfaces();
            builder.Register<DomainEventPublisher>().AsImplementedInterfaces();

            // AI Server Client
            builder.Register<AIAgentService>().AsImplementedInterfaces();

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
            builder.Register<GameInfoTools>();
            builder.Register<GoalInstantiator>();
            builder.Register<PlanConverter>();
        }
    }
}