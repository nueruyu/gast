using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.AI;
using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
{
    public class PlanConverter
    {
        readonly GoalInstantiator goalInstantiator;

        public PlanConverter(GoalInstantiator goalInstantiator)
        {
            this.goalInstantiator = goalInstantiator;
        }

        public List<IGoal> ToGoals(PlanDto plan)
        {
            if (plan?.Objectives == null)
            {
                return new List<IGoal>();
            }

            return plan.Objectives
                .OrderByDescending(o => o.Priority)
                .Select(TryConvertToGoal)
                .Where(g => g != null)
                .ToList();
        }

        IGoal TryConvertToGoal(ObjectiveDto objective)
        {
            try
            {
                return goalInstantiator.CreateGoal(objective.Type, objective.Parameters);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlanConverter] Failed to instantiate goal for objective '{objective.Type}': {ex.Message}");
                return null;
            }
        }
    }
}
