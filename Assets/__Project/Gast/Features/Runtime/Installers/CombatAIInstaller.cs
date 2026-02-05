using Gast.Core.DI;
using Gast.Features.AI.Combat;
using Gast.Features.AI.Combat.Actions;

namespace Gast.Features.Installers
{
    class CombatAIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<CombatDomain>(Lifetime.Transient);
            builder.Register<ChaseTargetAction>(Lifetime.Transient);
            builder.Register<MeleeAttackAction>(Lifetime.Transient);
            builder.Register<BackOffAction>(Lifetime.Transient);
            builder.Register<StrafeAction>(Lifetime.Transient);
            builder.Register<GuardAction>(Lifetime.Transient);
            builder.Register<StalkAction>(Lifetime.Transient);
            builder.Register<PostAttackManeuverAction>(Lifetime.Transient);
        }
    }
}