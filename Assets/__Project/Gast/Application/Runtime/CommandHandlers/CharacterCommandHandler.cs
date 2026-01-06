using Gast.Commands;
using Gast.Domain.Characters;
using Gast.UseCases.Characters;

namespace Gast.Api.CommandHandlers
{
    public class CharacterCommandHandler :
        ICommandHandler<CreatePlayerCommand, ICharacter>,
        ICommandHandler<CreateNpcCommand, ICharacter>
    {
        readonly CreatePlayerUseCase createPlayerUseCase;
        readonly CreateNpcUseCase createNpcUseCase;

        public CharacterCommandHandler(CreatePlayerUseCase createPlayerUseCase, CreateNpcUseCase createNpcUseCase)
        {
            this.createPlayerUseCase = createPlayerUseCase;
            this.createNpcUseCase = createNpcUseCase;
        }

        public ICharacter Execute(in CreatePlayerCommand command)
        {
            return createPlayerUseCase.Execute(command.TypeId, command.Position, command.Rotation);
        }

        public ICharacter Execute(in CreateNpcCommand command)
        {
            return createNpcUseCase.Execute(command.TypeId, command.Position, command.Rotation, command.Faction);
        }
    }
}