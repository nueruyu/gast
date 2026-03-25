using System;
using Cryst.Features.CharacterAI.Common.Actions;
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
            var root = builder.DefineCompound("Root");

            RegisterProcess(root, builder,
                "ReturningHome",
                AIMode.ReturningToHome,
                s => s.IsOutOfOuterTerritory,
                s => s.IsOutOfInnerTerritory);

            RegisterProcess(root, builder,
                "ThreatCombat",
                AIMode.Combat,
                s => s.IsThreatened);

            RegisterProcess(root, builder,
                "ObjectiveCombat",
                AIMode.Combat,
                s => s.HasObjectiveCombatTarget);

            RegisterProcess(root, builder,
                "Gathering",
                AIMode.Gathering,
                s => s.HasGatheringTarget);

            var idleProcess = DefineProcess(builder, "Idle", AIMode.Idle, _ => true);
            root.AddMethod("Handle_Default")
                .Do(idleProcess);

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> GetDomain()
        {
            return domain;
        }

        void RegisterProcess(
            CompoundTaskBuilder<ActorContext<StrategicState>, StrategicState> root,
            AIDomainBuilder<ActorContext<StrategicState>, StrategicState> builder,
            string name,
            AIMode mode,
            Func<StrategicState, bool> startCondition,
            Func<StrategicState, bool> continueCondition = null)
        {
            continueCondition ??= startCondition;

            var processTask = DefineProcess(builder, name, mode, continueCondition);

            root.AddMethod($"Handle_{name}")
                .When(startCondition)
                .Do(processTask);
        }

        CompoundTaskBuilder<ActorContext<StrategicState>, StrategicState> DefineProcess(
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