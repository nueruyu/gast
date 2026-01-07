using Gast.Api.AI;

namespace Gast.Features.Npcs
{
    public struct StrategicWorldState
    {
        public IGoal CurrentGoal { get; set; }
        public bool HasGoal { get; set; }
    }
}
