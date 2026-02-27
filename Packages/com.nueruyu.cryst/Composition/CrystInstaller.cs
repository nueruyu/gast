using Gast.Core.DI;
using Cryst.Features.CharacterAI.Combat;
using Cryst.Features.CharacterAI.Combat.Actions;
using Cryst.Features.CharacterAI.Gathering;
using Cryst.Features.CharacterAI.Gathering.Actions;
using Cryst.Features.CharacterAI.Strategic.Actions;
using UnityEngine;
using Cryst.Infrastructure.CharacterAI;
using Cryst.Infrastructure.Characters;
using Cryst.Features.Players;
using Cryst.Features.CharacterAI;
using Cryst.Features.CharacterAI.Strategic;
using Gast.Domain.Characters;
using Gast.Domain.Players;
using Gast.Core.Tasks;
using Cryst.Features.Characters.Feedbacks;
using Cryst.Features.Characters.EventHandlers;
using Cryst.Features.CharacterActions;
using Cryst.UI.Hud.Objectives;
using Cryst.Infrastructure.UI;
using Cryst.Infrastructure.Reflection;
using Gast.Application.AI;
using Cryst.Infrastructure;
using Cryst.Features.AI.Tools;
using Cryst.UI.Hud.PlayerStatus;
using Gast.Unity.Features.Characters;
using Gast.Unity.Infrastructure.Reflection;
using Gast.Unity.Shared.DI;
using Gast.Unity.UI.Hud.Objectives;
using Gast.Unity.UI.Hud.PlayerStatus;

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

            // Reflection
            builder.Register<CrystReflectionAssemblyProvider>().As<IReflectionAssemblyProvider>();

            // AI Tools
            builder.Register<GameInfoTools>(Lifetime.Singleton).As<IToolSet>();

            // Character
            builder.Register<CharacterActionFactory>().As<ICharacterActionFactory>();
            builder.Register<CharacterFactory>(Lifetime.Singleton).As<ICharacterFactory>();

            // Character Actions (Transient)
            builder.Register<AttackAction>(Lifetime.Transient);
            builder.Register<DashAction>(Lifetime.Transient);
            builder.Register<Features.CharacterActions.Default.DefaultAction>(Lifetime.Transient);
            builder.Register<DieAction>(Lifetime.Transient);
            builder.Register<Features.CharacterActions.GuardAction>(Lifetime.Transient);
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

            // Feedbacks
            builder.Register<CharacterFeedbackService>(Lifetime.Singleton);

            // Event Handling
            builder.Register<EventBindingRunner>(Lifetime.Singleton).As<ILifecycleTask>();
            builder.Register<HitFeedbackHandler>(Lifetime.Singleton);
            builder.Register<HitEffectHandler>(Lifetime.Singleton);
            builder.Register<CharacterDecompositionHandler>(Lifetime.Singleton);

            // Combat AI
            builder.Register<CombatDomain>(Lifetime.Transient);
            builder.Register<ChaseTargetAction>(Lifetime.Transient);
            builder.Register<MeleeAttackAction>(Lifetime.Transient);
            builder.Register<BackOffAction>(Lifetime.Transient);
            builder.Register<StrafeAction>(Lifetime.Transient);
            builder.Register<Features.CharacterAI.Combat.Actions.GuardAction>(Lifetime.Transient);
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