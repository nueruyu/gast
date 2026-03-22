using Cryst.Domain.AI.Objectives;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public interface IObjectiveQueries
    {
        IActorInfo FindBestTargetFor(DefeatCharacterObjective objective, ObjectiveState state);
        IPickupInfo FindBestTargetFor(AcquireItemObjective objective, ObjectiveState state);
    }
}
