using System.Threading;
using System.Threading.Tasks;
using Gast.Core.Commands;
using Gast.Domain.AI;
using Gast.Domain.Players;
using UnityEngine;

namespace Gast.Application.AIPlanning
{
    public class CommandAIUseCase : IAsyncCommandHandler<CommandAICommand, CommandAIResult>
    {
        readonly IAIPlanningService aiAgentService;
        readonly IPlayerManager playerManager;
        readonly ICharacterAIBrainFactory aiBrainFactory;

        public CommandAIUseCase(
            IAIPlanningService aiAgentService,
            IPlayerManager playerManager,
            ICharacterAIBrainFactory aiBrainFactory)
        {
            this.aiAgentService = aiAgentService;
            this.playerManager = playerManager;
            this.aiBrainFactory = aiBrainFactory;
        }

        public async ValueTask<CommandAIResult> ExecuteAsync(CommandAICommand command, CancellationToken cancellationToken)
        {
            var result = await aiAgentService.GetObjectivesAsync(command.Instruction, cancellationToken);

            if (!result.IsSuccess)
            {
                return CommandAIResult.Failure(result.Error.Message);
            }

            if (result.Objectives.Count == 0)
            {
                return CommandAIResult.Failure("AI server returned no goals");
            }

            var brain = aiBrainFactory.Create();
            brain.SetObjectives(result.Objectives);

            if (!playerManager.TakeoverWithAI(brain))
            {
                return CommandAIResult.Failure("Failed to takeover player character");
            }

            Debug.Log($"[CommandAIUseCase] AI took over player with {result.Objectives.Count} goals");
            return CommandAIResult.Success(result.Objectives.Count);
        }
    }

    public readonly struct CommandAIResult
    {
        public bool IsSuccess { get; }
        public int GoalCount { get; }
        public string ErrorMessage { get; }

        CommandAIResult(bool isSuccess, int goalCount, string errorMessage)
        {
            IsSuccess = isSuccess;
            GoalCount = goalCount;
            ErrorMessage = errorMessage;
        }

        public static CommandAIResult Success(int goalCount)
            => new(true, goalCount, null);

        public static CommandAIResult Failure(string errorMessage)
            => new(false, 0, errorMessage);
    }
}