using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Economy;
using Gast.Domain.Players;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gast.Unity.Features.Placement
{
    /// <summary>
    /// Debug input handler to trigger placement mode with F5 key.
    /// This should be replaced with proper UI integration later.
    /// </summary>
    public class PlacementInputHandler : ILifecycleTask
    {
        readonly PlacementService placementService;
        readonly IPlayerManager playerManager;
        readonly IItemRepository itemRepository;

        public PlacementInputHandler(
            PlacementService placementService,
            IPlayerManager playerManager,
            IItemRepository itemRepository)
        {
            this.placementService = placementService;
            this.playerManager = playerManager;
            this.itemRepository = itemRepository;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Keyboard.current != null && Keyboard.current.f5Key.wasPressedThisFrame)
                {
                    if (placementService.IsActive.Value)
                    {
                        placementService.ExitMode();
                    }
                    else
                    {
                        TryEnterPlacementMode();
                    }
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        void TryEnterPlacementMode()
        {
            var player = playerManager.CurrentCharacter.Value;
            if (player == null || !player.Is(out IInventoryHost inventoryHost))
            {
                return;
            }

            var placeableItem = inventoryHost.Inventory.Items
                .Select(itemStack => itemRepository.Get(itemStack.ItemId))
                .FirstOrDefault(itemDef => itemDef.Placeable);

            if (placeableItem != null)
            {
                placementService.EnterMode(placeableItem.Id);
            }
            else
            {
                Debug.Log("[PlacementInputHandler] No placeable items in inventory.");
            }
        }
    }
}
