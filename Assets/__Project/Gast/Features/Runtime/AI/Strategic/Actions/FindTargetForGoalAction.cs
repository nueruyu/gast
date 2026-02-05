using Cysharp.Threading.Tasks;
using Gast.Domain.AI.Objectives;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.AI.Strategic.Actions
{
    [Serializable]
    public class FindTargetForGoalAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        readonly ICharacterRepository characterRepository;

        public FindTargetForGoalAction(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public bool CanExecute(StrategicState worldState)
        {
            return worldState.HasGoal && worldState.CurrentGoal is DefeatCharacterObjective;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            if (ctx.WorldState.CurrentGoal is DefeatCharacterObjective defeatGoal)
            {
                var foundTarget = FindClosestCharacterOfType(ctx.Actor, defeatGoal.TargetTypeId);
                ctx.Memory.CombatTarget = foundTarget;
            }
            else
            {
                // Should not happen due to CanExecute, but as a safeguard.
                ctx.Memory.CombatTarget = null;
            }

            return UniTask.CompletedTask;
        }

        private ICharacter FindClosestCharacterOfType(ICharacter self, CharacterTypeId typeId)
        {
            return characterRepository.GetAll()
                .Where(c => c.TypeId == typeId && c.IsAlive)
                .Where(c => c.Status.Faction != self.Status.Faction)
                .OrderBy(c => Vector3.Distance(self.Body.Position, c.Body.Position))
                .FirstOrDefault();
        }
    }
}