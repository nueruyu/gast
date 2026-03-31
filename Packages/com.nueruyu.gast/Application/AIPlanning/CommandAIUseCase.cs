using System.Collections.Generic;
using System.Linq;
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
        readonly IEnumerable<ISlashCommandHandler> slashHandlers;

        public CommandAIUseCase(
            IAIPlanningService aiAgentService,
            IPlayerManager playerManager,
            ICharacterAIBrainFactory aiBrainFactory,
            IEnumerable<ISlashCommandHandler> slashHandlers)
        {
            this.aiAgentService = aiAgentService;
            this.playerManager = playerManager;
            this.aiBrainFactory = aiBrainFactory;
            this.slashHandlers = slashHandlers;
        }

        public async ValueTask<CommandAIResult> ExecuteAsync(CommandAICommand command, CancellationToken cancellationToken)
        {
            foreach (var handler in slashHandlers)
            {
                var prefix = handler.Prefix + " ";
                if (command.Instruction.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                {
                    var slashInstruction = command.Instruction[prefix.Length..];
                    return await handler.HandleAsync(slashInstruction, cancellationToken);
                }
            }

            AIPlanningResult result;
            try
            {
                result = await aiAgentService.GetObjectivesAsync(command.Instruction, cancellationToken);
            }
            catch (AIPlanningException ex)
            {
                return CommandAIResult.Failure(ex.Message);
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
        public string Message { get; }

        CommandAIResult(bool isSuccess, int goalCount, string message)
        {
            IsSuccess = isSuccess;
            GoalCount = goalCount;
            Message = message;
        }

        public static CommandAIResult Success(int goalCount, string message = null)
            => new(true, goalCount, message);

        public static CommandAIResult Failure(string message)
            => new(false, 0, message);
    }
}
