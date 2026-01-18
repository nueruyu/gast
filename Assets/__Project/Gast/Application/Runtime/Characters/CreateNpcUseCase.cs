using Gast.Api.Characters;
using Gast.Core.Commands;
using Gast.Domain.AI;
using Gast.Domain.Characters;

namespace Gast.Application.UseCases.Characters
{
    /// <summary>
    /// Use case for creating an AI-controlled (NPC) character.
    /// It spawns a character and attaches an appropriate brain from the factory.
    /// </summary>
    public class CreateNpcUseCase : ICommandHandler<CreateNpcCommand, ICharacter>
    {
        readonly SpawnCharacterUseCase spawnCharacterUseCase;
        readonly ICharacterAIBrainFactory brainFactory;

        public CreateNpcUseCase(SpawnCharacterUseCase spawnCharacterUseCase, ICharacterAIBrainFactory brainFactory)
        {
            this.spawnCharacterUseCase = spawnCharacterUseCase;
            this.brainFactory = brainFactory;
        }

        public ICharacter Execute(in CreateNpcCommand command)
        {
            var character = spawnCharacterUseCase.Execute(
                command.TypeId,
                command.Position,
                command.Rotation,
                command.Faction);

            var brain = brainFactory.Create();
            character.AttachBrain(brain);
            return character;
        }
    }
}