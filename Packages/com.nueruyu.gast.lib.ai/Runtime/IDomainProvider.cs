namespace Gast.Lib.AI
{
    public interface IDomainProvider<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        AIDomain<TActorContext, TWorldState> GetDomain();
    }
}
