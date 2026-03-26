using System;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Lib.AI;
using Newtonsoft.Json;

namespace Cryst.Features.Stories.Actions
{
    public class WaitForItemAcquiredActionFactory : IStoryActionFactory
    {
        public string ActionName => "WaitForItemAcquired";

        public IAction<StoryActorContext, StoryWorldState> Create(string parametersJson)
        {
            var p = JsonConvert.DeserializeObject<Params>(parametersJson);
            var acquirerId = CharacterId.FromGuid(Guid.Parse(p.AcquirerCharacterId));
            var itemId = ItemId.FromGuid(Guid.Parse(p.ItemId));
            return new WaitForItemAcquiredAction(acquirerId, itemId);
        }

        class Params
        {
            [JsonProperty("acquirer_character_id")]
            public string AcquirerCharacterId { get; set; }

            [JsonProperty("item_id")]
            public string ItemId { get; set; }
        }
    }
}
