using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Infrastructure.Settings;
using Gast.Lib.Gaia.Dto;

namespace Gast.Infrastructure.Remoting.AI
{
    public class MockAIPlanningService : IAIPlanningService
    {
        readonly MockAIPlanningSettings settings;
        readonly PlanConverter planConverter;

        public MockAIPlanningService(MockAIPlanningSettings settings, PlanConverter planConverter)
        {
            this.settings = settings;
            this.planConverter = planConverter;
        }

        public Task<AIPlanningResult> GetObjectivesAsync(string instruction, CancellationToken cancellationToken)
        {
            var mockPlan = new PlanDto
            {
                Objectives = settings.MockObjectives.Select(o => new ObjectiveDto
                {
                    Type = o.Type,
                    Priority = o.Priority,
                    Parameters = o.Parameters.ToDictionary(
                        p => p.Key,
                        p => (object)ParseValue(p.Value))
                }).ToList()
            };

            var goals = planConverter.ToGoals(mockPlan);
            var result = new AIPlanningResult(goals);

            return Task.FromResult(result);
        }

        private object ParseValue(string value)
        {
            if (int.TryParse(value, out var intValue))
            {
                return intValue;
            }
            return value;
        }
    }
}
