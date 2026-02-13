namespace Gast.Features.Characters
{
    /// <summary>
    /// Hook interface called during character creation to allow external modules
    /// to register services into the CharacterContext's service container.
    /// </summary>
    public interface ICharacterContextInitializer
    {
        void Initialize(CharacterContext context);
    }
}