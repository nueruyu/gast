using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Players;
using UnityEngine;

namespace Gast.Application.AI
{
    public class CommandAIUseCase
    {
        readonly IAIAgentService aiAgentService;
        readonly IPlayerManager playerManager;
        readonly ICharacterAIBrainFactory aiBrainFactory;

        public CommandAIUseCase(
            IAIAgentService aiAgentService,
            IPlayerManager playerManager,
            ICharacterAIBrainFactory aiBrainFactory)
        {
            this.aiAgentService = aiAgentService;
            this.playerManager = playerManager;
            this.aiBrainFactory = aiBrainFactory;
        }

        public async UniTask<CommandAIResult> Execute(string instruction, CancellationToken cancellationToken)
        {
            var result = await aiAgentService.GetGoalsAsync(instruction, cancellationToken);

            if (!result.IsSuccess)
            {
                return CommandAIResult.Failure(result.Error.Message);
            }

            if (result.Goals.Count == 0)
            {
                return CommandAIResult.Failure("AI server returned no goals");
            }

            var brain = aiBrainFactory.Create();
            brain.SetGoals(result.Goals);

            if (!playerManager.TakeoverWithAI(brain))
            {
                return CommandAIResult.Failure("Failed to takeover player character");
            }

            Debug.Log($"[CommandAIUseCase] AI took over player with {result.Goals.Count} goals");
            return CommandAIResult.Success(result.Goals.Count);
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