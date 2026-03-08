namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatDomainDefinition : IAIDomainDefinition
    {
        readonly IAIDomainFactory<CombatState> factory;
        readonly IWorldStateUpdater<CombatState> updater;

        public CombatDomainDefinition(IAIDomainFactory<CombatState> factory, IWorldStateUpdater<CombatState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void RegisterTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Combat",
                factory.CreateDomain(),
                new CombatState(),
                updater.Update);
        }
    }
}
