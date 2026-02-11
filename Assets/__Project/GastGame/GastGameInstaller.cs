using Gast.Core.DI;
using Gast.Shared.DI;
using GastGame.Features.AI.Combat;
using GastGame.Features.AI.Combat.Actions;
using GastGame.Features.AI.Gathering;
using GastGame.Features.AI.Gathering.Actions;
using GastGame.Features.AI.Strategic.Actions;
using UnityEngine;
using GastGame.Infrastructure.AI;
using GastGame.Infrastructure.Characters;
using GastGame.Features.Players;
using GastGame.Features.AI;
using GastGame.Features.AI.Strategic;

namespace GastGame
{
    [CreateAssetMenu(fileName = "GastGameInstaller", menuName = "Gast/Game/GastGame Installer")]
    public class GastGameInstaller : InstallerAsset
    {
        public override void Install(IContainerBuilder builder)
        {
            // Aspects
            builder.Register<GameCharacterFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            // AI Brain
            builder.Register<ObjectiveManager>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);
            builder.Register<CharacterAIBrainFactory>().AsImplementedInterfaces();

            // Player
            builder.Register<PlayerBrain>();
            builder.Register<PlayerManager>().AsImplementedInterfaces().AsSelf();

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