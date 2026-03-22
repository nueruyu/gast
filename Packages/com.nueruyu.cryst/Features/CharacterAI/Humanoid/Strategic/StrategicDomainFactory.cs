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

            // === Process: Combat ===
            var processCombat = builder.DefineCompound("ProcessCombat");
            processCombat.AddMethod("InitializeCombat")
                .When(s => s.CurrentMode != AIMode.Combat)
                .Do(new SelectThreatAction())
                .Do(new SetAIModeAction(AIMode.Combat));
            processCombat.AddMethod("EndCombat")
                .When(s => !s.IsThreatened)
                .Do(new ClearObjectiveAction())
                .Do(new SetAIModeAction(AIMode.Idle));
            processCombat.AddMethod("ContinueCombat")
                .Do(new IdleAction());

            // === Process: ReturningHome ===
            var processReturningHome = builder.DefineCompound("ProcessReturningHome");
            processReturningHome.AddMethod("InitializeReturningHome")
                .When(s => s.CurrentMode != AIMode.ReturningToHome)
                .Do(new ClearObjectiveAction())
                .Do(new SetAIModeAction(AIMode.ReturningToHome));
            processReturningHome.AddMethod("ArrivedHome")
                .When(s => !s.IsOutOfTerritoryCore)
                .Do(new SetAIModeAction(AIMode.Idle));
            processReturningHome.AddMethod("ContinueReturning")
                .Do(new IdleAction());

            // === Process: Default Behavior (Idle/Objective-driven) ===
            var processDefault = builder.DefineCompound("ProcessDefault");
            processDefault.AddMethod("StartNewObjective")
                .When(s => s.HasObjective)
                .Do(new SetAIModeFromObjectiveAction());
            processDefault.AddMethod("ReturnToIdle")
                .When(s => s.CurrentMode != AIMode.Idle)
                .Do(new ClearObjectiveAction())
                .Do(new SetAIModeAction(AIMode.Idle));
            processDefault.AddMethod("ContinueIdle")
                .Do(new IdleAction());


            // === Root: Dispatcher ===
            var root = builder.DefineCompound("Root");

            root.AddMethod("Handle_ReturningHome")
                .When(s => s.IsOutOfTerritory)
                .Do(processReturningHome);

            root.AddMethod("Handle_Combat")
                .When(s => s.IsThreatened)
                .Do(processCombat);

            root.AddMethod("Handle_Default")
                .Do(processDefault);

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> CreateDomain()
        {
            return domain;
        }
    }
}
