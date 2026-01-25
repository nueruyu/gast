using System.Collections.Generic;
using System.Linq;
using Gast.Application.AI.Attributes;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Application.AI.Tools
{
    public class GameInfoTools
    {
        readonly ICharacterTypeRepository characterTypeRepository;
        readonly IItemRepository itemRepository;

        public GameInfoTools(
            ICharacterTypeRepository characterTypeRepository,
            IItemRepository itemRepository)
        {
            this.characterTypeRepository = characterTypeRepository;
            this.itemRepository = itemRepository;
        }

        [Tool(
            "GetCharacterTypes",
            "Get a list of available character types and their definitions.")]
        public List<object> GetCharacterTypes()
        {
            return characterTypeRepository.GetAllDefinitions()
                .Select(def => new
                {
                    Id = def.TypeId.ToString(),
                    Name = def.DisplayName,
                    def.MaxHealth,
                    def.CanGuard
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
    }
}