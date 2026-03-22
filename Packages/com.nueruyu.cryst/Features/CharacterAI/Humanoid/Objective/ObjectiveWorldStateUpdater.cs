namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveWorldStateUpdater : IWorldStateUpdater<ObjectiveState>
    {
        public void Update(ActorContext<ObjectiveState> context)
        {
            context.WorldState.Update(context);
        }
    }
}
