namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatDomainConstruct
    {
        readonly IAIDomainFactory<CombatState> factory;
        readonly IWorldStateUpdater<CombatState> updater;

        public CombatDomainConstruct(IAIDomainFactory<CombatState> factory, IWorldStateUpdater<CombatState> updater)
        {
            this.factory = factory;
            this.updater = updater;
        }

        public void ApplyTo(IDomainRegistrar registrar)
        {
            registrar.Register(
                "Combat",
                factory,
                new CombatState(),
                updater.Update);
        }
    }
}
