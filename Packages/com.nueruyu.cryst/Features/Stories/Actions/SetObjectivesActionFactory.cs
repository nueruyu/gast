using System;
using System.Collections.Generic;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Newtonsoft.Json;

namespace Cryst.Features.Stories.Actions
{
    public class SetObjectivesActionFactory : IStoryActionFactory
    {
        public string ActionName => "SetObjectives";

        public IAction<StoryActorContext, StoryWorldState> Create(string parametersJson)
        {
            var p = JsonConvert.DeserializeObject<Params>(parametersJson);
            var assignments = new List<SetObjectivesAction.Assignment>();

            foreach (var raw in p.Assignments)
            {
                assignments.Add(new SetObjectivesAction.Assignment
                {
                    CharacterId = CharacterId.FromGuid(Guid.Parse(raw.CharacterId)),
                    ObjectiveType = raw.ObjectiveType,
                    Parameters = raw.ObjectiveParameters ?? new Dictionary<string, object>()
                });
            }

            return new SetObjectivesAction(assignments);
        }

        class Params
        {
            [JsonProperty("assignments")]
            public List<AssignmentJson> Assignments { get; set; }
        }

        class AssignmentJson
        {
            [JsonProperty("character_id")]
            public string CharacterId { get; set; }

            [JsonProperty("objective_type")]
            public string ObjectiveType { get; set; }

            [JsonProperty("objective_parameters")]
            public Dictionary<string, object> ObjectiveParameters { get; set; }
        }
    }
}
