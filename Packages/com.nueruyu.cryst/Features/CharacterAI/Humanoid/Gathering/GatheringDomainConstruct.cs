namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringDomainConstruct
    {
        readonly IAIDomainFactory<GatheringState> factory;
        readonly IWorldStateUpdater<GatheringState> updater;

        public GatheringDomainConstruct(IAIDomainFactory<GatheringState> factory, IWorldStateUpdater<GatheringState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void ApplyTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Gathering",
                factory,
                new GatheringState(),
                updater.Update);
        }
    }
}
