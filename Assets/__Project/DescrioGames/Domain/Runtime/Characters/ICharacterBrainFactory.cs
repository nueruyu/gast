namespace DescrioGames.Domain.Characters
{
    public interface ICharacterBrainFactory
    {
        ICharacterBrain Create(CharacterTypeId typeId);
    }
}