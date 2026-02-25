namespace Gast.Features.Characters
{
    public interface ICharacterActionFactory
    {
        ICharacterAction Create(CharacterActionSettings settings, CharacterContext context);
    }
}