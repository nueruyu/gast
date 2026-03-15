using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolState : IWorldState<PatrolState>
    {
        public AIMode CurrentMode { get; set; }
        public bool IsOutOfTerritory { get; set; }

        public void WriteTo(ref PatrolState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.IsOutOfTerritory = IsOutOfTerritory;
        }
    }
}
