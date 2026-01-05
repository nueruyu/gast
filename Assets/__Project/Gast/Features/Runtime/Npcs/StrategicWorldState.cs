using System.Collections.Generic;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Npcs.Goals;

namespace Gast.Features.Npcs
{
    public struct StrategicWorldState
    {
        public IReadOnlyList<IGoal> Goals { get; set; }
        public IGoal CurrentGoal { get; set; }

        // Information about the current target derived from the goal
        public CharacterId TargetToDefeatId { get; set; }

        public readonly bool HasTargetToDefeat => TargetToDefeatId != default;

        public ItemId TargetToAcquireId { get; set; }
        public readonly bool HasTargetToAcquire => TargetToAcquireId != default;
    }
}