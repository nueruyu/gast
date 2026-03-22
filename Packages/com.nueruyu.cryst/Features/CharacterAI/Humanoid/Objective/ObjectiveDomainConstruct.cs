namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveDomainConstruct
    {
        readonly IAIDomainFactory<ObjectiveState> factory;
        readonly IWorldStateUpdater<ObjectiveState> updater;

        public ObjectiveDomainConstruct(IAIDomainFactory<ObjectiveState> factory, IWorldStateUpdater<ObjectiveState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void ApplyTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Objective",
                factory.CreateDomain(),
                new ObjectiveState(),
                updater.Update);
        }
    }
}
