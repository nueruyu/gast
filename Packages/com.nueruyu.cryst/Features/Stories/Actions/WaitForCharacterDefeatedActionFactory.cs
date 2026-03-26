using System;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Newtonsoft.Json;

namespace Cryst.Features.Stories.Actions
{
    public class WaitForCharacterDefeatedActionFactory : IStoryActionFactory
    {
        public string ActionName => "WaitForCharacterDefeated";

        public IAction<StoryActorContext, StoryWorldState> Create(string parametersJson)
        {
            var p = JsonConvert.DeserializeObject<Params>(parametersJson);
            var characterId = CharacterId.FromGuid(Guid.Parse(p.CharacterId));
            return new WaitForCharacterDefeatedAction(characterId);
        }

        class Params
        {
            [JsonProperty("character_id")]
            public string CharacterId { get; set; }
        }
    }
}
