using System.Collections.Generic;
using System.Linq;
using Gast.Application.AI.Tools;
using Gast.Infrastructure.Repositories;

namespace Gast.Infrastructure.AI.Tools
{
    public class GameInfoTools
    {
        readonly CharacterTypeRepository characterTypeRepository;
        readonly ItemRepository itemRepository;

        public GameInfoTools(
            CharacterTypeRepository characterTypeRepository,
            ItemRepository itemRepository)
        {
            this.characterTypeRepository = characterTypeRepository;
            this.itemRepository = itemRepository;
        }

        [AITool("GetCharacterTypes", "Get a list of available character types and their definitions.")]
        public List<object> GetCharacterTypes()
        {
            return characterTypeRepository.GetAllDefinitions()
                .Select(def => new
                {
                    id = def.TypeId.ToString(),
                    name = def.DisplayName,
                    max_health = def.MaxHealth,
                    can_guard = def.CanGuard
                })
                .Cast<object>()
                .ToList();
        }

        [AITool("GetItemTypes", "Get a list of all defined items.")]
        public List<object> GetItemTypes()
        {
            return itemRepository.GetAllDefinitions()
                .Select(def => new
                {
                    id = def.Id.ToString(),
                    name = def.Name,
                    price = def.Price,
                    description = def.Description
                })
                .Cast<object>()
                .ToList();
        }
    }
}
