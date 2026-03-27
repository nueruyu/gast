using System;
using System.Collections.Generic;
using System.Linq;
using Cryst.Features.Stories.Converters;
using Gast.Domain.Characters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Cryst.Features.Stories.Actions
{
    public class SetObjectivesActionFactory : StoryActionFactory<SetObjectivesActionFactory.Params>
    {
        readonly IReadOnlyDictionary<string, IStoryObjectiveFactory> objectiveRegistry;
        readonly JsonSerializer objectiveSerializer;

        public SetObjectivesActionFactory(IEnumerable<IStoryObjectiveFactory> objectiveFactories)
        {
            objectiveRegistry = objectiveFactories.ToDictionary(
                f => f.ObjectiveType,
                f => f,
                StringComparer.OrdinalIgnoreCase);

            objectiveSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters =
                {
                    new CharacterIdJsonConverter(),
                    new CharacterTypeIdJsonConverter()
                }
            });
        }

        public override string ActionName => "SetObjectives";

        protected override SetObjectivesAction Create(Params parameters)
        {
            var assignments = parameters.Assignments.Select(def =>
            {
                if (!objectiveRegistry.TryGetValue(def.ObjectiveType, out var factory))
                    throw new NotSupportedException(
                        $"[SetObjectivesActionFactory] Unknown objective type '{def.ObjectiveType}'.");

                var objectiveParams = (def.ObjectiveParameters ?? new JObject())
                    .ToObject(factory.ParameterType, objectiveSerializer);
                var objective = factory.Create(objectiveParams);
                return new SetObjectivesAction.Assignment(def.CharacterId, objective);
            }).ToList();

            return new SetObjectivesAction(assignments);
        }

        public class Params
        {
            public List<AssignmentDef> Assignments { get; set; }
        }

        public class AssignmentDef
        {
            public CharacterId CharacterId { get; set; }
            public string ObjectiveType { get; set; }
            public JObject ObjectiveParameters { get; set; }
        }
    }
}
