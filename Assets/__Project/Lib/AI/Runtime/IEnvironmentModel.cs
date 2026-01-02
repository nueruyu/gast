namespace DescrioGames.Lib.AI
{
    public interface IEnvironmentModel<TWorldState> where TWorldState : struct
    {
        void Simulate(ref TWorldState state, ISimulationContext context);
    }
}