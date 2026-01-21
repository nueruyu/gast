using Gast.Domain.AI;
using Gast.Lib.AI;

namespace Gast.Features.AI.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public IGoal CurrentGoal { get; set; }
        public bool HasGoal { get; set; }
        public bool IsThreatened { get; set; }

        public void CopyFrom(StrategicState source)
        {
            CurrentGoal = source.CurrentGoal;
            HasGoal = source.HasGoal;
            IsThreatened = source.IsThreatened;
        }
    }
}
