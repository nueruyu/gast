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

namespace Cryst.Composition
{
    [CreateAssetMenu(fileName = "CrystInstaller", menuName = "Cryst/Installer")]
    public class CrystInstaller : InstallerAsset
    {
        public override void Install(IContainerBuilder builder)
        {
            // Character
            builder.Register<CharacterContextInitializer>().AsImplementedInterfaces();

            // Aspects
            builder.Register<CrystCharacterFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            // AI Brain
            builder.Register<ObjectiveManager>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);
            builder.Register<CharacterAIBrainFactory>().AsImplementedInterfaces();

            // Player
            builder.Register<PlayerCharacterController>().As<IPlayerCharacterController>();

            // Combat AI
            builder.Register<CombatDomain>(Lifetime.Transient);
            builder.Register<ChaseTargetAction>(Lifetime.Transient);
            builder.Register<MeleeAttackAction>(Lifetime.Transient);
            builder.Register<BackOffAction>(Lifetime.Transient);
            builder.Register<StrafeAction>(Lifetime.Transient);
            builder.Register<GuardAction>(Lifetime.Transient);
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