using System.Threading;
using System.Threading.Tasks;
using Gast.Core.Commands;

namespace Gast.Application.AIPlanning
{
    public class StorySlashCommandHandler : ISlashCommandHandler
    {
        readonly ICommandDispatcher commandDispatcher;

        public string Prefix => "/story";

        public StorySlashCommandHandler(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public async ValueTask<CommandAIResult> HandleAsync(string instruction, CancellationToken cancellationToken)
        {
            var success = await commandDispatcher.DispatchAsync<GenerateStoryCommand, bool>(
                new GenerateStoryCommand(instruction), cancellationToken);

            return success
                ? CommandAIResult.Success(0, "Story generation started.")
                : CommandAIResult.Failure("Story generation failed.");
        }
    }
}
