using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.AI;
using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class PlanConverter
    {
        readonly ObjectiveTypeResolver typeResolver;
        readonly JsonSerializer serializer;

        public PlanConverter(ObjectiveTypeResolver typeResolver)
        {
            this.typeResolver = typeResolver;
            serializer = JsonSerializer.Create(GastJsonSettings.Create());
        }

        public List<IAIObjective> ToGoals(PlanDto plan)
        {
            if (plan?.Objectives == null)
            {
                return new List<IAIObjective>();
            }

            return plan.Objectives
                .OrderByDescending(o => o.Priority)
                .Select(TryConvertToGoal)
                .Where(g => g != null)
                .ToList();
        }

        IAIObjective TryConvertToGoal(ObjectiveDto objective)
        {
            try
            {
                var type = typeResolver.Resolve(objective.Type);
                var jObject = JObject.FromObject(objective.Parameters);
                return (IAIObjective)jObject.ToObject(type, serializer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlanConverter] Failed to instantiate goal for objective '{objective.Type}': {ex.Message}");
                Debug.LogException(ex);
                return null;
            }
        }
    }
}
