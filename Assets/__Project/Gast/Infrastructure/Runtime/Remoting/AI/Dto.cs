using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gast.Infrastructure.Remoting.AI
{
    // --- Request DTOs ---

    public class PlanRequestDto
    {
        [JsonProperty("context")]
        public AgentContextDto Context { get; set; }

        [JsonProperty("definitions")]
        public StaticDefinitionsDto Definitions { get; set; }

        [JsonProperty("available_goals")]
        public List<GoalDefinitionDto> AvailableGoals { get; set; }
    }

    public class AgentContextDto
    {
        [JsonProperty("agent_character_type")]
        public string AgentCharacterType { get; set; }

        [JsonProperty("mission_objective")]
        public string MissionObjective { get; set; }
    }

    public class StaticDefinitionsDto
    {
        [JsonProperty("character_types")]
        public List<CharacterTypeDto> CharacterTypes { get; set; }

        [JsonProperty("item_types")]
        public List<ItemDto> ItemTypes { get; set; }
    }

    public class CharacterTypeDto
    {
        [JsonProperty("type_id")]
        public string TypeId { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("threat_level")]
        public int ThreatLevel { get; set; }
    }

    public class ItemDto
    {
        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("utility")]
        public int Utility { get; set; }
    }

    public class GoalDefinitionDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; }
    }

    // --- Response DTOs ---

    public class PlanResponseDto
    {
        [JsonProperty("overall_objective")]
        public string OverallObjective { get; set; }

        [JsonProperty("objectives")]
        public List<ObjectiveDto> Objectives { get; set; }

        [JsonProperty("thought")]
        public string Thought { get; set; }
    }

    public class ObjectiveDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }
    }
}
