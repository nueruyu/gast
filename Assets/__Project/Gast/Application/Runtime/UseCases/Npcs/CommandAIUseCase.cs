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

        public async UniTask Execute(string instruction, CancellationToken cancellationToken)
        {
            var goals = await aiAgentService.GetGoalsAsync(instruction, cancellationToken);

            if (goals.Count == 0)
            {
                Debug.LogWarning("AI server returned no goals.");
                return;
            }

            // Assign goals to the first available NPC with a goal-assignable brain
            var npc = characterRepository.GetAll()
                .FirstOrDefault(c => c.GetBrain() is IGoalAssignable);

            if (npc != null)
            {
                var brain = npc.GetBrain() as IGoalAssignable;
                brain?.SetGoals(goals);
                Debug.Log($"Assigned {goals.Count} goals to NPC {npc.Id}.");
            }
            else
            {
                Debug.LogWarning("No suitable NPC with IGoalAssignable brain found to assign goals.");
            }
        }
    }
}