using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;
using Gast.Unity.Features.Stories.Actions;
using Gast.Unity.Infrastructure.JsonConverters;
using Gast.Unity.Infrastructure.Remoting.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Unity.Infrastructure.Stories.Factories
{
    public class SetObjectivesActionFactory : IStoryActionFactory<SetObjectivesActionFactory.Params>
    {
        readonly ObjectiveTypeResolver typeResolver;
        readonly JsonSerializer objectiveSerializer;

        public SetObjectivesActionFactory(ObjectiveTypeResolver typeResolver)
        {
            this.typeResolver = typeResolver;
            objectiveSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters =
                {
                    new CharacterIdJsonConverter(),
                    new CharacterTypeIdJsonConverter(),
                    new ItemIdJsonConverter()
                }
            });
        }

        public string ActionName => "SetObjectives";

        public IAction<StoryActorContext, StoryWorldState> Create(Params parameters)
        {
            var assignments = parameters.Assignments.Select(def =>
            {
                var type = typeResolver.Resolve(def.ObjectiveType);
                var jObject = JObject.FromObject(def.ObjectiveParameters ?? new Dictionary<string, object>());
                var objective = (IAIObjective)jObject.ToObject(type, objectiveSerializer);

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
            public Dictionary<string, object> ObjectiveParameters { get; set; }
        }
    }
}
