using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolState : IWorldState<PatrolState>
    {
        public bool IsActive { get; private set; }
        public bool IsOutOfOuterTerritory { get; private set; }
        public bool IsOutOfInnerTerritory { get; private set; }

        public void WriteTo(ref PatrolState dest)
        {
            dest ??= new();
            dest.IsActive = IsActive;
            dest.IsOutOfOuterTerritory = IsOutOfOuterTerritory;
            dest.IsOutOfInnerTerritory = IsOutOfInnerTerritory;
        }

        public void EnterTerritory()
        {
            IsOutOfOuterTerritory = false;
        }

        public void Update(bool isActive, bool isOutOfOuterTerritory, bool isOutOfInnerTerritory)
        {
            IsActive = isActive;
            IsOutOfOuterTerritory = isOutOfOuterTerritory;
            IsOutOfInnerTerritory = isOutOfInnerTerritory;
        }
    }
}