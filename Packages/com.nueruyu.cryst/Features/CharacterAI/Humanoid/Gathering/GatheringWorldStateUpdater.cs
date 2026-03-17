namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringWorldStateUpdater : IWorldStateUpdater<GatheringState>
    {
        public void Update(ActorContext<GatheringState> context)
        {
            context.WorldState.Update(context);
        }
    }
}