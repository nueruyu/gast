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

            root.AddMethod("SetMode_ReturnToHome")
                .When(s => s.IsOutOfTerritory)
                .Do(new ClearTargetAction())
                .Do(new SetAIModeAction(AIMode.ReturningToHome));
            root.AddMethod("SetMode_Combat")
                .When(s => s.IsThreatened)
                .Do(new SelectThreatAction())
                .Do(new SetAIModeAction(AIMode.Combat));
            root.AddMethod("SetMode_FromObjective")
                .When(s => s.AvailableObjectives.Count > 0)
                .Do(new SelectObjectiveAction())
                .Do(new SetAIModeFromObjectiveAction());
            root.AddMethod("SetMode_Idle")
                .Do(new ClearTargetAction())
                .Do(new SetAIModeAction(AIMode.Idle));
            root.AddMethod("Wait")
                .Do(new WaitAction(new FloatRange(0.2f, 0.3f)));

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> CreateDomain()
        {
            return domain;
        }
    }
}