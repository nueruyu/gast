using Cryst.Domain.AI.Objectives;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public interface IObjectiveQueries
    {
        ActorInfo? FindBestTargetFor(DefeatCharacterObjective objective, ObjectiveState state);
        PickupInfo? FindBestTargetFor(AcquireItemObjective objective, ObjectiveState state);
    }
}
