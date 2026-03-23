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

        public void Update(ActorContext<PatrolState> context)
        {
            var aiModeMemory = context.GetModule<AIModeMemory>();
            var character = context.Character;

            IsActive = aiModeMemory.CurrentMode == AIMode.ReturningToHome;

            if (character.Is(out TerritorialCharacter territorial))
            {
                if (IsOutOfTerritory)
                    IsOutOfTerritory = !territorial.HasReturnedToTerritory();
                else
                    IsOutOfTerritory = territorial.IsOutOfTerritory();
            }
            else
            {
                IsOutOfTerritory = false;
            }
        }
    }
}