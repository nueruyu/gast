using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gast.Infrastructure.Remoting
{
    // Data sent to the AI server
    public class AiRequest
    {
        [JsonProperty("instruction")]
        public string Instruction { get; set; }

        [JsonProperty("character_types")]
        public List<CharacterTypeDto> CharacterTypes { get; set; }

        [JsonProperty("items")]
        public List<ItemDto> Items { get; set; }
    }

    public class CharacterTypeDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ItemDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    // Data received from the AI server
    public class AiResponse
    {
        [JsonProperty("output")]
        public AiResponseOutput Output { get; set; }
    }

    public class AiResponseOutput
    {
        [JsonProperty("goals")]
        public List<GoalDto> Goals { get; set; }
    }

    public class GoalDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("character_type_id")]
        public string CharacterTypeId { get; set; }

        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
