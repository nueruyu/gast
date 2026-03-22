namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveDomainConstruct
    {
        readonly IWorldStateUpdater<ObjectiveState> updater;

        public ObjectiveDomainConstruct(IWorldStateUpdater<ObjectiveState> updater)
        {
            this.updater = updater;
        }

        public void ApplyTo(IDomainRegistrar registrar, ObjectiveManager objectiveManager)
        {
            var factory = new ObjectiveDomainFactory(objectiveManager);
            registrar.Register(
                "Objective",
                factory,
                new ObjectiveState(),
                updater.Update);
        }
    }
}
