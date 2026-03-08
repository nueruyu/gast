using System.Collections.Generic;
using Gast.Domain.AI;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public bool IsThreatened { get; set; }
        public List<IAIObjective> AvailableObjectives { get; set; } = new();

        public void WriteTo(ref StrategicState dest)
        {
            dest ??= new();
            dest.IsThreatened = IsThreatened;
            dest.AvailableObjectives = AvailableObjectives;
        }
    }
}