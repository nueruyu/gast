using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Api.AI;
using Gast.Application.Services;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Application.UseCases.Npcs
{
    public class CommandAIUseCase
    {
        readonly IAIAgentService aiAgentService;
        readonly ICharacterRepository characterRepository;

        public CommandAIUseCase(IAIAgentService aiAgentService, ICharacterRepository characterRepository)
        {
            this.aiAgentService = aiAgentService;
            this.characterRepository = characterRepository;
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

            // Assign goals to the first available NPC with a goal-assignable brain
            var npc = characterRepository.GetAll()
                .FirstOrDefault(c => c.GetBrain() is IGoalAssignable);

            if (npc == null)
            {
                return CommandAIResult.Failure("No suitable NPC found to assign goals");
            }

            var brain = npc.GetBrain() as IGoalAssignable;
            brain?.SetGoals(result.Goals);

            Debug.Log($"[CommandAIUseCase] Assigned {result.Goals.Count} goals to NPC {npc.Id}");
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