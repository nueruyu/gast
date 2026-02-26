using Gast.Core.Commands;
using Gast.Domain.AI;
using Gast.Domain.Characters;

namespace Gast.Application.Characters
{
    /// <summary>
    /// Use case for creating an AI-controlled (NPC) character.
    /// It spawns a character and attaches an appropriate brain from the factory.
    /// </summary>
    public class CreateNpcUseCase : ICommandHandler<CreateNpcCommand, ICharacter>
    {
        readonly SpawnCharacterUseCase spawnCharacterUseCase;
        readonly ICharacterAIBrainFactory brainFactory;
        readonly ICharacterBrainManager brainManager;

        public CreateNpcUseCase(
            SpawnCharacterUseCase spawnCharacterUseCase,
            ICharacterAIBrainFactory brainFactory,
            ICharacterBrainManager brainManager)
        {
            this.spawnCharacterUseCase = spawnCharacterUseCase;
            this.brainFactory = brainFactory;
            this.brainManager = brainManager;
        }

        public ICharacter Execute(in CreateNpcCommand command)
        {
            var character = spawnCharacterUseCase.Execute(
                command.Position,
                command.Rotation,
                command.Parameters);

            var brain = brainFactory.Create();
            brainManager.AttachBrain(character.Id, brain);
            return character;
        }
    }
}
