using Gast.Core.DI;
using GastGame.AI;
using GastGame.AI.Combat;
using GastGame.AI.Combat.Actions;
using GastGame.AI.Gathering;
using GastGame.AI.Gathering.Actions;
using GastGame.AI.Strategic;
using GastGame.AI.Strategic.Actions;
using GastGame.Factories;
using GastGame.Players;
using UnityEngine;

namespace GastGame
{
    [CreateAssetMenu(fileName = "GastGameInstaller", menuName = "Gast/Game/GastGame Installer")]
    public class GastGameInstaller : InstallerAsset
    {
        public override void Install(IContainerBuilder builder)
        {
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
