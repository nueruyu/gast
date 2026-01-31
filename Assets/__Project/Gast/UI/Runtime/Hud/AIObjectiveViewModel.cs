using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;
using Gast.Domain.AI.Objectives;
using System.Reflection;

namespace Gast.UI.Hud
{
    public class AIObjectiveViewModel
    {
        public string Description { get; }
        public string ProgressText { get; }
        public bool IsCompleted { get; }

        public AIObjectiveViewModel(IAIObjective objective)
        {
            IsCompleted = objective.IsCompleted;
            Description = GetObjectiveDescription(objective);
            ProgressText = GetProgressText(objective);
        }

        private string GetObjectiveDescription(IAIObjective objective)
        {
            var attr = objective.GetType().GetCustomAttribute<AIObjectiveAttribute>();
            return attr?.Description ?? objective.GetType().Name;
        }

        private string GetProgressText(IAIObjective objective)
        {
            if (objective is AcquireItemObjective acquire)
            {
                return $"{acquire.CurrentQuantity} / {acquire.TargetQuantity}";
            }
            if (objective is DefeatCharacterObjective defeat)
            {
                return $"{defeat.CurrentQuantity} / {defeat.TargetQuantity}";
            }
            return "";
        }
    }
}
