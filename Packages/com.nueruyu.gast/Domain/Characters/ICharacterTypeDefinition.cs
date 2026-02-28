namespace Gast.Domain.Characters
{
    public interface ICharacterTypeDefinition
    {
        CharacterTypeId TypeId { get; }
        string DisplayName { get; }
        bool TryGetSettings<T>(out T value);
    }
}
