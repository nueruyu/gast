namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolWorldStateUpdater : IWorldStateUpdater<PatrolState>
    {
        public void Update(ActorContext<PatrolState> context)
        {
            context.WorldState.Update(context);
        }
    }
}