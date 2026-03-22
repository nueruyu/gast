using System;
using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicDomainFactory : IAIDomainFactory<StrategicState>
    {
        readonly AIDomain<ActorContext<StrategicState>, StrategicState> domain;

        public StrategicDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<StrategicState>, StrategicState>();

            var processThreatCombat = DefineProcess(
                builder,
                "ThreatCombat",
                AIMode.Combat,
                s => s.IsThreatened);

            var processObjectiveCombat = DefineProcess(
                builder,
                "ObjectiveCombat",
                AIMode.Combat,
                s => s.HasObjectiveCombatTarget);

            var processReturningHome = DefineProcess(
                builder,
                "ReturningHome",
                AIMode.ReturningToHome,
                s => s.IsOutOfTerritoryCore);

            var processGathering = DefineProcess(
                builder,
                "Gathering",
                AIMode.Gathering,
                s => s.HasGatheringObjective);

            var processIdle = DefineProcess(
                builder,
                "Idle",
                AIMode.Idle,
                _ => true);

            // === Root: Dispatcher ===
            var root = builder.DefineCompound("Root");

            root.AddMethod("Handle_ReturningHome")
                .When(s => s.IsOutOfTerritory)
                .Do(processReturningHome);

            root.AddMethod("Handle_ThreatCombat")
                .When(s => s.IsThreatened)
                .Do(processThreatCombat);

            root.AddMethod("Handle_CombatObjective")
                .When(s => s.HasObjectiveCombatTarget)
                .Do(processObjectiveCombat);

            root.AddMethod("Handle_Gathering")
                .When(s => s.HasGatheringObjective)
                .Do(processGathering);

            root.AddMethod("Handle_Default")
                .Do(processIdle);

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> CreateDomain()
        {
            return domain;
        }

        static CompoundTaskBuilder<ActorContext<StrategicState>, StrategicState> DefineProcess(
            AIDomainBuilder<ActorContext<StrategicState>, StrategicState> builder,
            string name,
            AIMode mode,
            Func<StrategicState, bool> continueCondition)
        {
            var process = builder.DefineCompound($"Process{name}");
            process.AddMethod($"Initialize{name}")
                .When(s => s.CurrentMode != mode)
                .Do(new SetAIModeAction(mode));
            process.AddMethod($"End{name}")
                .When(s => !continueCondition(s))
                .Do(new SetAIModeAction(AIMode.Idle));
            process.AddMethod($"Continue{name}")
                .Do(new IdleAction());
            return process;
        }
    }
}