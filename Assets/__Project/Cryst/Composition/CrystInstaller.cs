using Gast.Core.DI;
using Gast.Shared.DI;
using Cryst.Modules.CharacterAI.Combat;
using Cryst.Modules.CharacterAI.Combat.Actions;
using Cryst.Modules.CharacterAI.Gathering;
using Cryst.Modules.CharacterAI.Gathering.Actions;
using Cryst.Modules.CharacterAI.Strategic.Actions;
using UnityEngine;
using Cryst.Infrastructure.CharacterAI;
using Cryst.Infrastructure.Characters;
using Cryst.Modules.Players;
using Cryst.Modules.CharacterAI;
using Cryst.Modules.CharacterAI.Strategic;
using Gast.Domain.Players;
using Gast.Core.Tasks;
using Cryst.Infrastructure.Feedbacks;
using Cryst.Infrastructure.EventHandlers;
using Gast.Features.Characters;
using Cryst.Modules.CharacterActions;
using Gast.UI.Hud.Status;
using Cryst.UI.Hud.Status;
using Gast.UI.Hud.Objectives;
using Cryst.UI.Hud.Objectives;
using Cryst.Infrastructure.UI;

namespace Cryst.Composition
{
    [CreateAssetMenu(fileName = "CrystInstaller", menuName = "Cryst/Installer")]
    public class CrystInstaller : InstallerAsset
    {
        [SerializeField]
        HitFeedbackSettings meleeAttackEffectSettings;

        public override void Install(IContainerBuilder builder)
        {
            // Settings
            builder.RegisterInstance(meleeAttackEffectSettings);

            // Character
            builder.Register<CharacterContextInitializer>().AsImplementedInterfaces();
            builder.Register<CharacterActionFactory>().As<ICharacterActionFactory>();
            builder.Register<CrystCharacterFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            // Character Actions (Transient)
            builder.Register<AttackAction>(Lifetime.Transient);
            builder.Register<DashAction>(Lifetime.Transient);
            builder.Register<Modules.CharacterActions.Default.DefaultAction>(Lifetime.Transient);
            builder.Register<DieAction>(Lifetime.Transient);
            builder.Register<Modules.CharacterActions.GuardAction>(Lifetime.Transient);
            builder.Register<HitAction>(Lifetime.Transient);
            builder.Register<JumpAction>(Lifetime.Transient);

            // AI Brain
            builder.Register<ObjectiveManager>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);
            builder.Register<CharacterAIBrainFactory>().AsImplementedInterfaces();

            // UI
            builder.Register<PlayerStatusViewModel>(Lifetime.Singleton);
            builder.Register<PlayerStatusViewFactory>(Lifetime.Singleton).As<IPlayerStatusViewFactory>();
            builder.Register<AcquireItemObjectiveViewModel>(Lifetime.Transient);
            builder.Register<DefeatCharacterObjectiveViewModel>(Lifetime.Transient);
            builder.Register<AIObjectiveViewModelFactory>(Lifetime.Singleton).As<IAIObjectiveViewModelFactory>();

            // Player
            builder.Register<PlayerCharacterController>().As<IPlayerCharacterController>();

            // Event Handlers
            builder.Register<HitFeedbackHandler>(Lifetime.Singleton).As<ILifecycleTask>();
            builder.Register<HitEffectHandler>(Lifetime.Singleton).As<ILifecycleTask>();
            builder.Register<CharacterDecompositionHandler>(Lifetime.Singleton).As<ILifecycleTask>();

            // Combat AI
            builder.Register<CombatDomain>(Lifetime.Transient);
            builder.Register<ChaseTargetAction>(Lifetime.Transient);
            builder.Register<MeleeAttackAction>(Lifetime.Transient);
            builder.Register<BackOffAction>(Lifetime.Transient);
            builder.Register<StrafeAction>(Lifetime.Transient);
            builder.Register<Modules.CharacterAI.Combat.Actions.GuardAction>(Lifetime.Transient);
            builder.Register<StalkAction>(Lifetime.Transient);
            builder.Register<PostAttackManeuverAction>(Lifetime.Transient);

            // Strategic AI
            builder.Register<StrategicDomain>(Lifetime.Transient);
            builder.Register<SelectObjectiveAction>(Lifetime.Transient);
            builder.Register<SelectThreatAction>(Lifetime.Transient);
            builder.Register<ClearTargetAction>(Lifetime.Transient);

            // Gathering AI
            builder.Register<GatheringDomain>(Lifetime.Transient);
            builder.Register<FindItemPickupAction>(Lifetime.Transient);
            builder.Register<MoveToInteractableAction>(Lifetime.Transient);
            builder.Register<InteractWithTargetAction>(Lifetime.Transient);
            builder.Register<ClearInteractableTargetAction>(Lifetime.Transient);
        }
    }
}