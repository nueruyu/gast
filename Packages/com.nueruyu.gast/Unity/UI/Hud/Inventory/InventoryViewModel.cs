using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Application.Equipment;
using Gast.Domain.Economy;
using Gast.Domain.Players;
using Gast.Unity.Shared.Observables;
using R3;

namespace Gast.Unity.UI.Hud.Inventory
{
    public class InventoryViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();
        readonly IPlayerManager playerManager;
        readonly EquipItemUseCase equipUseCase;

        public int HotbarSize => 10;

        public ReadOnlyReactiveProperty<IReadOnlyList<ItemStackViewModel>> InventoryItems { get; }

        public InventoryViewModel(
            IPlayerManager playerManager,
            ItemStackViewModelFactory itemStackViewModelFactory,
            EquipItemUseCase equipUseCase)
        {
            this.playerManager = playerManager;
            this.equipUseCase = equipUseCase;

            InventoryItems = playerManager.CurrentCharacter.ToObservable()
                .Select(character =>
                {
                    if (character == null || !character.Is(out IInventoryHost inventoryHost))
                    {
                        return Observable.Return((IReadOnlyList<ItemStackViewModel>)Array.Empty<ItemStackViewModel>());
                    }

                    return inventoryHost.Inventory.InventoryChanged.ToObservable()
                        .Prepend(Unit.Default)
                        .Select(_ => (IReadOnlyList<ItemStackViewModel>)inventoryHost.Inventory.Items
                            .Select(itemStackViewModelFactory.Create)
                            .Take(HotbarSize)
                            .ToArray());
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void EquipItem(ItemId itemId)
        {
            var character = playerManager.CurrentCharacter.Value;
            if (character == null) return;
            equipUseCase.Execute(new EquipItemCommand(character.Id, itemId));
        }

        public string GetSlotNumberText(int index)
        {
            if (HotbarSize == 10 && index == 9) return "0";
            return (index + 1).ToString();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
