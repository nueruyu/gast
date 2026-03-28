using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;
using Gast.Unity.Features.Stories.Actions;
using Gast.Unity.Infrastructure.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gast.Unity.Infrastructure.Stories.Factories
{
    public class SetObjectivesActionFactory : IStoryActionFactory<SetObjectivesActionFactory.Params>
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

        public string ActionName => "SetObjectives";

        public IAction<StoryActorContext, StoryWorldState> Create(Params parameters)
        {
            var assignments = parameters.Assignments.Select(def =>
            {
                if (!objectiveRegistry.TryGetValue(def.ObjectiveType, out var factory))
                    throw new NotSupportedException(
                        $"[SetObjectivesActionFactory] Unknown objective type '{def.ObjectiveType}'.");

                var objectiveParamsJson = def.ObjectiveParameters ?? "{}";
                using var reader = new JsonTextReader(new StringReader(objectiveParamsJson));
                var objectiveParams = objectiveSerializer.Deserialize(reader, factory.ParameterType);
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

            [JsonConverter(typeof(RawJsonStringConverter))]
            public string ObjectiveParameters { get; set; }
        }
    }
}