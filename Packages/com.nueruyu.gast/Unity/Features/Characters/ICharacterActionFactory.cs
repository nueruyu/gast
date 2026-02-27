namespace Gast.Unity.Features.Characters
{
    public interface ICharacterActionFactory
    {
        ICharacterAction Create(CharacterActionSettings settings, CharacterContext context);
    }
}