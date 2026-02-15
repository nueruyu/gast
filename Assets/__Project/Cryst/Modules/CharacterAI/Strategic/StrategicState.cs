using Gast.Domain.AI;
using Gast.Lib.AI;
using System.Collections.Generic;

namespace Cryst.Modules.CharacterAI.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public bool IsThreatened { get; set; }
        public List<IAIObjective> AvailableObjectives { get; set; } = new();

        public void CopyFrom(StrategicState source)
        {
            IsThreatened = source.IsThreatened;
            AvailableObjectives = source.AvailableObjectives;
        }
    }
}