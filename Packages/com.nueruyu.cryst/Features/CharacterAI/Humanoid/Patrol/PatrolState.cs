using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolState : IWorldState<PatrolState>
    {
        public bool IsActive { get; private set; }
        public bool IsOutOfTerritory { get; private set; }

        public void WriteTo(ref PatrolState dest)
        {
            dest ??= new();
            dest.IsActive = IsActive;
            dest.IsOutOfTerritory = IsOutOfTerritory;
        }

        public void EnterTerritory()
        {
            IsOutOfTerritory = false;
        }

        public void Update(bool isActive, bool isOutOfTerritory)
        {
            IsActive = isActive;

            // If we are already out of territory, we stay out until we have returned.
            // This prevents flipping back and forth if the condition logic is complex.
            if (IsOutOfTerritory)
                IsOutOfTerritory = isOutOfTerritory;
            else
                IsOutOfTerritory = isOutOfTerritory;
        }
    }
}
