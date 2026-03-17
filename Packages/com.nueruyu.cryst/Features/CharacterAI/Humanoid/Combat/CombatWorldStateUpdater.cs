namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatWorldStateUpdater : IWorldStateUpdater<CombatState>
    {
        public void Update(ActorContext<CombatState> context)
        {
            context.WorldState.Update(context);
        }
    }
}