using Gast.Core.DI;
using UnityEngine;
using Cryst.Infrastructure.CharacterAI;
using Cryst.Infrastructure.Characters;
using Cryst.Features.Players;
using Cryst.Features.CharacterAI;
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
using Cryst.Features.CharacterActions.Actions.Attack;
using Cryst.Features.CharacterActions.Actions.Dash;
using Cryst.Features.CharacterActions.Actions.Default;
using Cryst.Features.CharacterActions.Actions.Die;
using Cryst.Features.CharacterActions.Actions.Hit;
using Cryst.Features.CharacterActions.Actions.Jump;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterActions.Effects;
using Cryst.Features.CharacterAI.Humanoid.Combat;
using Cryst.Features.CharacterAI.Humanoid.Gathering;
using Cryst.Features.CharacterAI.Humanoid.Strategic;
using Cryst.UI.Hud.PlayerStatus;
using Gast.Application.Reflection;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.DI;
using Gast.Unity.UI.Hud.Objectives;
using Gast.Unity.UI.Hud.PlayerStatus;
using GuardAction = Cryst.Features.CharacterActions.Actions.Guard.GuardAction;

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
            builder.Register<DefaultAction>(Lifetime.Transient);
            builder.Register<DieAction>(Lifetime.Transient);
            builder.Register<GuardAction>(Lifetime.Transient);
            builder.Register<HitAction>(Lifetime.Transient);
            builder.Register<JumpAction>(Lifetime.Transient);

            // Character Action Effects
            builder.Register<PlaySoundEffectHandler>(Lifetime.Transient).As<ICharacterActionEffectHandler>();
            builder.Register<SpawnHitAreaEffectHandler>(Lifetime.Transient).As<ICharacterActionEffectHandler>();
            builder.Register<SpawnVfxEffectHandler>(Lifetime.Transient).As<ICharacterActionEffectHandler>();

            // Character Facets (Transient)
            builder.Register<AttackableCharacter>(Lifetime.Transient);
            builder.Register<DashableCharacter>(Lifetime.Transient);
            builder.Register<GuardableCharacter>(Lifetime.Transient);
            builder.Register<JumpableCharacter>(Lifetime.Transient);
            builder.Register<SprintableCharacter>(Lifetime.Transient);

            // AI Brain
            builder.Register<AIBrainServices>(Lifetime.Singleton);
            builder.Register<ObjectiveManager>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);
            builder.Register<CharacterAIBrainFactory>().AsImplementedInterfaces();

            // AI Domains
            builder.Register<CombatDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<CombatState>>();
            builder.Register<StrategicDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<StrategicState>>();
            builder.Register<GatheringDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<GatheringState>>();

            builder.Register<CombatWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<CombatState>>();
            builder.Register<StrategicWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<StrategicState>>();
            builder.Register<GatheringWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<GatheringState>>();

            builder.Register<CombatDomainDefinition>(Lifetime.Singleton).As<IAIDomainDefinition>();
            builder.Register<StrategicDomainDefinition>(Lifetime.Singleton).As<IAIDomainDefinition>();
            builder.Register<GatheringDomainDefinition>(Lifetime.Singleton).As<IAIDomainDefinition>();

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
        }
    }
}
