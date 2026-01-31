using Gast.Application.Economy;
using Gast.Domain.AI;
using Gast.Domain.AI.Objectives;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.UI.Hud
{
    public class AIObjectiveViewModelFactory
    {
        readonly IItemRepository itemRepository;
        readonly IItemAssetService itemAssetService;
        readonly ICharacterTypeRepository characterTypeRepository;

        public AIObjectiveViewModelFactory(
            IItemRepository itemRepository,
            IItemAssetService itemAssetService,
            ICharacterTypeRepository characterTypeRepository)
        {
            this.itemRepository = itemRepository;
            this.itemAssetService = itemAssetService;
            this.characterTypeRepository = characterTypeRepository;
        }

        public IAIObjectiveViewModel Create(IAIObjective objective)
        {
            if (objective is AcquireItemObjective acquire)
            {
                var icon = itemAssetService.GetItemIcon(acquire.TargetItemId);
                return new AcquireItemObjectiveViewModel(acquire, itemRepository, icon);
            }

            if (objective is DefeatCharacterObjective defeat)
            {
                return new DefeatCharacterObjectiveViewModel(defeat, characterTypeRepository);
            }

            return new FallbackObjectiveViewModel(objective);
        }
    }
}
