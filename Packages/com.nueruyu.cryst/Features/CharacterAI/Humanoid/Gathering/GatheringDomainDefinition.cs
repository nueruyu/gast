namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringDomainDefinition
    {
        readonly IAIDomainFactory<GatheringState> factory;
        readonly IWorldStateUpdater<GatheringState> updater;

        public GatheringDomainDefinition(IAIDomainFactory<GatheringState> factory, IWorldStateUpdater<GatheringState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void RegisterTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Gathering",
                factory.CreateDomain(),
                new GatheringState(),
                updater.Update);
        }
    }
}
