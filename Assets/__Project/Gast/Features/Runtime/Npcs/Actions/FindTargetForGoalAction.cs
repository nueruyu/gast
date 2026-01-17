using Cysharp.Threading.Tasks;
using Gast.Api.AI.Goals;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class FindTargetForGoalAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly ICharacterRepository characterRepository;

        public FindTargetForGoalAction(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public string Name => "FindTargetForGoalAction";

        public bool CanExecute(StrategicWorldState worldState) => worldState.HasGoal;

        public void Simulate(StrategicWorldState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var goal = ctx.WorldState.CurrentGoal;
            ICharacter foundTarget = null;

            if (goal is DefeatCharacterGoal defeatGoal)
            {
                foundTarget = FindClosestCharacterOfType(ctx.Actor, defeatGoal.TargetTypeId);
            }

            ctx.SharedState.CombatTarget = foundTarget;
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
