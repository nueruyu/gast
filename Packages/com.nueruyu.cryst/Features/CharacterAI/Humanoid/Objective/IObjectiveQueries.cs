using Gast.Domain.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public interface IObjectiveQueries
    {
        (IAIObjective objective, IActorInfo target) FindBestCombatObjective(ObjectiveState state);
        (IAIObjective objective, IPickupInfo target) FindBestGatheringObjective(ObjectiveState state);
    }
}
