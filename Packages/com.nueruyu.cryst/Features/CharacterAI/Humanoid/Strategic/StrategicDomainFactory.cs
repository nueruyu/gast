using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Strategic.Actions;
using Gast.Core.Values;
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

            root.AddMethod("TransitionTo_ReturningHome")
                .When(s => s.IsOutOfTerritory)
                .While(s => s.IsOutOfTerritoryCore)
                .Do(new ClearTargetAction())
                .Do(new SetAIModeAction(AIMode.ReturningToHome))
                .Do(new IdleAction());
            root.AddMethod("TransitionTo_Combat")
                .When(s => s.IsThreatened && s.CurrentMode != AIMode.Combat)
                .Do(new SelectThreatAction())
                .Do(new SetAIModeAction(AIMode.Combat));
            root.AddMethod("TransitionTo_ObjectiveSeeking")
                .When(s => s.AvailableObjectives.Count > 0)
                .Do(new SelectObjectiveAction())
                .Do(new SetAIModeFromObjectiveAction())
                .Do(new WaitAction(0.5f));
            root.AddMethod("TransitionTo_Idle")
                .When(s =>
                    s.CurrentMode != AIMode.Idle &&
                    !s.IsThreatened &&
                    s.AvailableObjectives.Count == 0)
                .Do(new ClearTargetAction())
                .Do(new SetAIModeAction(AIMode.Idle));
            root.AddMethod("Maintain_CurrentMode")
                .Do(new WaitAction(new FloatRange(0.2f, 0.3f)));

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> CreateDomain()
        {
            return domain;
        }
    }
}