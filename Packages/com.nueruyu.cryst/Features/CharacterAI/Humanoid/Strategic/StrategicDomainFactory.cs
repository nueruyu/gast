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

            root.AddMethod("ReturnToHome")
                .Condition(s => s.IsOutOfTerritory)
                .Do(new ReturnToHomeAction());
            root.AddMethod("RespondToThreat")
                .Condition(s => s.IsThreatened && !s.IsOutOfTerritory)
                .Do(new SelectThreatAction());
            root.AddMethod("PursueObjective")
                .Condition(s => !s.IsThreatened && !s.IsOutOfTerritory && s.AvailableObjectives.Count > 0)
                .Do(new SelectObjectiveAction());
            root.AddMethod("Idle")
                .Condition(s => !s.IsThreatened && !s.IsOutOfTerritory && s.AvailableObjectives.Count == 0)
                .Do(new ClearTargetAction())
                .Do(new WaitAction(new FloatRange(0.8f, 1.2f)));

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> CreateDomain()
        {
            return domain;
        }
    }
}