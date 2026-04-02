using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Domain.Players;
using Gast.Unity.Features.Equipment;
using Gast.Unity.Shared.Observables;
using Gast.Unity.UI.Hud.Inventory;
using R3;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();

        IEquipmentHost equipmentHost;

        public IReadOnlyList<EquipmentSlotViewModel> SlotViewModels { get; }

        public EquipmentViewModel(
            IPlayerManager playerManager,
            IEquipmentSlotProvider slotProvider,
            InventoryViewModel inventoryViewModel)
        {
            var characterStream = playerManager.CurrentCharacter.ToObservable();

            characterStream
                .Subscribe(c => equipmentHost = c != null && c.Is(out IEquipmentHost h) ? h : null)
                .AddTo(disposables);

            SlotViewModels = slotProvider.Slots.Select(def =>
            {
                var item = characterStream
                    .Select(c =>
                    {
                        if (c == null || !c.Is(out IEquipmentHost h)) return Observable.Return<ItemId?>(null);
                        return h.GetSlot(def.Id).ToObservable();
                    })
                    .Switch()
                    .ToReadOnlyReactiveProperty()
                    .AddTo(disposables);

                return new EquipmentSlotViewModel(def, item, slotId => equipmentHost?.Unequip(slotId));
            }).ToArray();

            inventoryViewModel.ItemSelected
                .Subscribe(EquipToFirstAvailableSlot)
                .AddTo(disposables);
        }

        void EquipToFirstAvailableSlot(ItemId itemId)
        {
            if (equipmentHost == null) return;

            foreach (var slotId in equipmentHost.Slots)
            {
                if (!equipmentHost.GetSlot(slotId).Value.HasValue)
                {
                    equipmentHost.Equip(slotId, itemId);
                    return;
                }
            }

            var first = equipmentHost.Slots.FirstOrDefault();
            equipmentHost.Equip(first, itemId);
        }

        public void RequestEquip(EquipmentSlotId slotId, ItemId itemId) =>
            equipmentHost?.Equip(slotId, itemId);

        public void Dispose() => disposables.Dispose();
    }
}
