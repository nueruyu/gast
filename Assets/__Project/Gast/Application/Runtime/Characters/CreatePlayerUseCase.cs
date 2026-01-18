using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Players;

namespace Gast.Application.Characters
{
    /// <summary>
    /// Use case for creating a player-controlled character.
    /// It spawns a character and uses the PlayerManager to possess it.
    /// </summary>
    public class CreatePlayerUseCase : ICommandHandler<CreatePlayerCommand, ICharacter>
    {
        readonly SpawnCharacterUseCase spawnCharacterUseCase;
        readonly IPlayerManager playerManager;

        public CreatePlayerUseCase(SpawnCharacterUseCase spawnCharacterUseCase, IPlayerManager playerManager)
        {
            this.spawnCharacterUseCase = spawnCharacterUseCase;
            this.playerManager = playerManager;
        }

        public ICharacter Execute(in CreatePlayerCommand command)
        {
            var character = spawnCharacterUseCase.Execute(
                command.TypeId,
                command.Position,
                command.Rotation,
                Faction.Player);

            playerManager.Possess(character.Id);
            return character;
        }
    }
}