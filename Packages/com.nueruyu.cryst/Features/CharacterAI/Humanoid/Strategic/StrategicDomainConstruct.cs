namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicDomainConstruct
    {
        readonly IAIDomainFactory<StrategicState> factory;
        readonly IWorldStateUpdater<StrategicState> updater;

        public StrategicDomainConstruct(IAIDomainFactory<StrategicState> factory, IWorldStateUpdater<StrategicState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void ApplyTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Strategic",
                factory.CreateDomain(),
                new StrategicState(),
                updater.Update);
        }
    }
}
