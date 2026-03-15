namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolDomainConstruct
    {
        readonly IAIDomainFactory<PatrolState> factory;
        readonly IWorldStateUpdater<PatrolState> updater;

        public PatrolDomainConstruct(IAIDomainFactory<PatrolState> factory, IWorldStateUpdater<PatrolState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void ApplyTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Patrol",
                factory.CreateDomain(),
                new PatrolState(),
                updater.Update);
        }
    }
}
