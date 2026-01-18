using Gast.Application.AI;
using Gast.Features.AI;
using Gast.Features.AI.Combat;
using Gast.Features.AI.Combat.Actions;
using Gast.Features.AI.Strategic;
using Gast.Features.AI.Strategic.Actions;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class AIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<GoalManager>(Lifetime.Transient);

            builder.Register<ChaseTargetAction>(Lifetime.Transient);
            builder.Register<MeleeAttackAction>(Lifetime.Transient);
            builder.Register<BackOffAction>(Lifetime.Transient);
            builder.Register<StrafeAction>(Lifetime.Transient);

            builder.Register<ClearTargetAction>(Lifetime.Transient);
            builder.Register<FindTargetForGoalAction>(Lifetime.Transient);
            builder.Register<SelectThreatAction>(Lifetime.Transient);

            builder.Register<FindItemPickupAction>(Lifetime.Transient);
            builder.Register<MoveToInteractableAction>(Lifetime.Transient);
            builder.Register<InteractWithTargetAction>(Lifetime.Transient);
            builder.Register<ClearInteractableTargetAction>(Lifetime.Transient);

            builder.Register<StrategicDomain>(Lifetime.Transient);
            builder.Register<CombatDomain>(Lifetime.Transient);

            builder.Register<AIBrain>(Lifetime.Transient);
        }
    }
}