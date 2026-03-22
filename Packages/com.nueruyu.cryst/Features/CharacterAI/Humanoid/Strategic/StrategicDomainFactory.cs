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

            // === Process: ThreatCombat ===
            var processThreatCombat = builder.DefineCompound("ProcessThreatCombat");
            processThreatCombat.AddMethod("InitializeThreatCombat")
                .When(s => s.CurrentMode != AIMode.Combat)
                .Do(new SetAIModeAction(AIMode.Combat));
            processThreatCombat.AddMethod("EndThreatCombat")
                .When(s => !s.IsThreatened)
                .Do(new ClearObjectiveAction())
                .Do(new SetAIModeAction(AIMode.Idle));
            processThreatCombat.AddMethod("ContinueThreatCombat")
                .Do(new IdleAction());

            // === Process: ObjectiveCombat ===
            var processObjectiveCombat = builder.DefineCompound("ProcessObjectiveCombat");
            processObjectiveCombat.AddMethod("InitializeObjectiveCombat")
                .When(s => s.CurrentMode != AIMode.Combat)
                .Do(new SetAIModeAction(AIMode.Combat));
            processObjectiveCombat.AddMethod("EndObjectiveCombat")
                .When(s => !s.HasObjectiveCombatTarget)
                .Do(new ClearObjectiveAction())
                .Do(new SetAIModeAction(AIMode.Idle));
            processObjectiveCombat.AddMethod("ContinueObjectiveCombat")
                .Do(new IdleAction());

            // === Process: ReturningHome ===
            var processReturningHome = builder.DefineCompound("ProcessReturningHome");
            processReturningHome.AddMethod("InitializeReturningHome")
                .When(s => s.CurrentMode != AIMode.ReturningToHome)
                .Do(new SetAIModeAction(AIMode.ReturningToHome));
            processReturningHome.AddMethod("ArrivedHome")
                .When(s => !s.IsOutOfTerritoryCore)
                .Do(new SetAIModeAction(AIMode.Idle));
            processReturningHome.AddMethod("ContinueReturning")
                .Do(new IdleAction());

            // === Process: Gathering ===
            var processGathering = builder.DefineCompound("ProcessGathering");
            processGathering.AddMethod("InitializeGathering")
                .When(s => s.CurrentMode != AIMode.Gathering)
                .Do(new SetAIModeAction(AIMode.Gathering));
            processGathering.AddMethod("EndGathering")
                .When(s => !s.HasGatheringObjective)
                .Do(new SetAIModeAction(AIMode.Idle));
            processGathering.AddMethod("ContinueGathering")
                .Do(new IdleAction());

            // === Process: Idle ===
            var processIdle = builder.DefineCompound("ProcessIdle");
            processIdle.AddMethod("InitializeIdle")
                .When(s => s.CurrentMode != AIMode.Idle)
                .Do(new SetAIModeAction(AIMode.Idle));
            processIdle.AddMethod("ContinueIdle")
                .Do(new IdleAction());

            // === Root: Dispatcher ===
            var root = builder.DefineCompound("Root");

            root.AddMethod("Handle_ReturningHome")
                .When(s => s.IsOutOfTerritory)
                .Do(processReturningHome);

            root.AddMethod("Handle_Threat")
                .When(s => s.IsThreatened && !s.HasThreatTarget)
                .Do(new SelectThreatAction());

            root.AddMethod("Handle_ThreatCombat")
                .When(s => s.HasThreatTarget)
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
    }
}