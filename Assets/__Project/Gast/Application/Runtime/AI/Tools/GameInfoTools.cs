using System.Collections.Generic;
using System.Linq;
using Gast.Application.AI.Attributes;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;

namespace Gast.Application.AI.Tools
{
    public class GameInfoTools
    {
        readonly ICharacterTypeRepository characterTypeRepository;
        readonly IItemRepository itemRepository;
        readonly IPickupRepository pickupRepository;

        public GameInfoTools(
            ICharacterTypeRepository characterTypeRepository,
            IItemRepository itemRepository,
            IPickupRepository pickupRepository)
        {
            this.characterTypeRepository = characterTypeRepository;
            this.itemRepository = itemRepository;
            this.pickupRepository = pickupRepository;
        }

        [Tool(
            "GetCharacterTypes",
            "Get a list of available character types, their stats, and potential loot drops.")]
        public List<object> GetCharacterTypes()
        {
            return characterTypeRepository.GetAllDefinitions()
                .Select(def => new
                {
                    Id = def.TypeId.ToString(),
                    Name = def.DisplayName,
                    def.CanGuard,
                    Loot = def.LootTable?.Entries.Select(entry =>
                    {
                        var itemDef = itemRepository.Get(entry.ItemId);
                        return new
                        {
                            ItemId = entry.ItemId.ToString(),
                            ItemName = itemDef?.Name ?? "Unknown",
                            Chance = entry.DropRate,
                            Min = entry.MinQuantity,
                            Max = entry.MaxQuantity
                        };
                    }).ToList()
                })
                .Cast<object>()
                .ToList();
        }

        [Tool(
            "GetItemTypes",
            "Get a list of all defined items.")]
        public List<object> GetItemTypes()
        {
            return itemRepository.GetAllDefinitions()
                .Select(def => new
                {
                    Id = def.Id.ToString(),
                    def.Name,
                    def.Price,
                    def.Description
                })
                .Cast<object>()
                .ToList();
        }

        [Tool(
            "GetWorldPickups",
            "Get a list of items currently dropped in the world that can be picked up.")]
        public List<object> GetWorldPickups()
        {
            return pickupRepository.GetAll()
                .Select(p =>
                {
                    var itemDef = itemRepository.Get(p.ItemId);
                    return new
                    {
                        Id = p.Id.ToString(),
                        ItemId = p.ItemId.ToString(),
                        ItemName = itemDef?.Name ?? "Unknown",
                        p.Quantity,
                        Position = new { p.Position.x, p.Position.y, p.Position.z }
                    };
                })
                .Cast<object>()
                .ToList();
        }
    }
}
