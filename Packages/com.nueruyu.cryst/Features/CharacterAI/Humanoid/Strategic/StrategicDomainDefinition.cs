namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicDomainDefinition : IAIDomainDefinition
    {
        readonly IAIDomainFactory<StrategicState> factory;
        readonly IWorldStateUpdater<StrategicState> updater;

        public StrategicDomainDefinition(IAIDomainFactory<StrategicState> factory, IWorldStateUpdater<StrategicState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void RegisterTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Strategic",
                factory.CreateDomain(),
                new StrategicState(),
                updater.Update);
        }
    }
}
