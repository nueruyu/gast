using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gast.Infrastructure.Remoting.AI
{
    // --- Attributes ---

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ObjectiveTypeAttribute : Attribute
    {
        public string TypeName { get; }
        public ObjectiveTypeAttribute(string typeName) => TypeName = typeName;
    }

    // --- Request DTOs ---

    public class PlanRequestDto
    {
        public AgentContextDto Context { get; set; }
        public StaticDefinitionsDto Definitions { get; set; }
        public List<GoalDefinitionDto> AvailableGoals { get; set; }
    }

    public class AgentContextDto
    {
        public string AgentCharacterType { get; set; }
        public string MissionObjective { get; set; }
    }

    public class StaticDefinitionsDto
    {
        public List<CharacterTypeDto> CharacterTypes { get; set; }
        public List<ItemDto> ItemTypes { get; set; }
    }

    public class CharacterTypeDto
    {
        public string TypeId { get; set; }
        public string DisplayName { get; set; }
        public int ThreatLevel { get; set; }
    }

    public class ItemDto
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public int Utility { get; set; }
    }

    public class GoalDefinitionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    // --- Response DTOs ---

    public class PlanResponseDto
    {
        public string OverallObjective { get; set; }
        public List<ObjectiveDto> Objectives { get; set; }
        public string Thought { get; set; }
    }

    /// <summary>
    /// Base class for objectives.
    /// The JsonConverter handles selecting the concrete class based on the "type" field
    /// mapped via [ObjectiveType] attributes.
    /// </summary>
    [JsonConverter(typeof(ObjectiveConverter))]
    public abstract class ObjectiveDto
    {
        public string Type { get; set; }
        public int Priority { get; set; }
    }

    [ObjectiveType("DefeatCharacter")]
    public class DefeatCharacterObjectiveDto : ObjectiveDto
    {
        public DefeatCharacterParametersDto Parameters { get; set; }
    }

    public class DefeatCharacterParametersDto
    {
        public string CharacterTypeId { get; set; }
        public int Quantity { get; set; }
    }

    [ObjectiveType("AcquireItem")]
    public class AcquireItemObjectiveDto : ObjectiveDto
    {
        public AcquireItemParametersDto Parameters { get; set; }
    }

    public class AcquireItemParametersDto
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Fallback for unknown objective types.
    /// </summary>
    public class UnknownObjectiveDto : ObjectiveDto
    {
        public Dictionary<string, object> Parameters { get; set; }
    }

    // --- Converters ---

    public class ObjectiveConverter : JsonConverter
    {
        static readonly Dictionary<string, Type> TypeMap;

        static ObjectiveConverter()
        {
            TypeMap = new Dictionary<string, Type>();

            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetCustomAttribute<ObjectiveTypeAttribute>() != null);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ObjectiveTypeAttribute>();
                if (attr != null && !string.IsNullOrEmpty(attr.TypeName))
                {
                    TypeMap[attr.TypeName] = type;
                }
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ObjectiveDto);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeToken = jsonObject["type"];
            var typeName = typeToken?.ToString();

            ObjectiveDto dto;

            if (!string.IsNullOrEmpty(typeName) && TypeMap.TryGetValue(typeName, out var concreteType))
            {
                dto = (ObjectiveDto)Activator.CreateInstance(concreteType);
            }
            else
            {
                dto = new UnknownObjectiveDto();
            }

            using (var subReader = jsonObject.CreateReader())
            {
                serializer.Populate(subReader, dto);
            }

            return dto;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
