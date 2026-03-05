using Cryst.Features.CharacterAI.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Strategic
{
    public class StrategicDomain
    {
        readonly AIDomain<StrategicState, AIContext<StrategicState>> domain;

        public StrategicDomain()
        {
            var builder = new AIDomainBuilder<StrategicState, AIContext<StrategicState>>();
            var selectObjective = builder.RegisterAction("SelectObjective", new SelectObjectiveAction());
            var selectThreat = builder.RegisterAction("SelectThreat", new SelectThreatAction());
            var clearTarget = builder.RegisterAction("ClearTarget", new ClearTargetAction());
            var wait = builder.RegisterAction<float>("Wait", new WaitAction());

            builder.DefineCompound("Root", c =>
            {
                c.AddMethod("RespondToThreat")
                    .Condition(s => s.IsThreatened)
                    .Do(selectThreat)
                    .End();
                c.AddMethod("PursueObjective")
                    .Condition(s => !s.IsThreatened && s.AvailableObjectives.Count > 0)
                    .Do(selectObjective)
                    .End();
                c.AddMethod("Idle")
                    .Condition(s => !s.IsThreatened && s.AvailableObjectives.Count == 0)
                    .Do(clearTarget)
                    .Do(wait, 1.0f)
                    .End();
            });

            domain = builder.Build("Root");
        }

        public AIRunner<StrategicState, AIContext<StrategicState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}
