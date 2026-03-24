namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringWorldStateUpdater : IWorldStateUpdater<GatheringState>
    {
        public void Update(ActorContext<GatheringState> context)
        {
            var aiModeMemory = context.GetModule<AIModeMemory>();
            var gatheringMemory = context.GetModule<GatheringMemory>();
            var actor = context.Actor;

            context.WorldState.Update(
                isActive: aiModeMemory.CurrentMode == AIMode.Gathering,
                currentTargetItemId: gatheringMemory.TargetItemId,
                interactableTarget: gatheringMemory.InteractableTarget,
                actorPosition: actor.Body.Position);
        }
    }
}
