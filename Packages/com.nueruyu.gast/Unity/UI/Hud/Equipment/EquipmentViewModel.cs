using System;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Domain.Players;
using Gast.Unity.UI.Hud.Inventory;
using R3;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();

        IEquipmentHost equipmentHost;

        public ReadOnlyReactiveProperty<ItemId?> HeadItem { get; }
        public ReadOnlyReactiveProperty<ItemId?> BodyItem { get; }
        public ReadOnlyReactiveProperty<ItemId?> WeaponItem { get; }

        public EquipmentViewModel(IPlayerManager playerManager, InventoryViewModel inventoryViewModel)
        {
            var characterStream = playerManager.CurrentCharacter.ToObservable();

            characterStream
                .Subscribe(c => equipmentHost = c != null && c.Is(out IEquipmentHost h) ? h : null)
                .AddTo(disposables);

            HeadItem = characterStream
                .Select(c =>
                {
                    if (c == null || !c.Is(out IEquipmentHost h)) return Observable.Return<ItemId?>(null);
                    return (Observable<ItemId?>)h.Head;
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            BodyItem = characterStream
                .Select(c =>
                {
                    if (c == null || !c.Is(out IEquipmentHost h)) return Observable.Return<ItemId?>(null);
                    return (Observable<ItemId?>)h.Body;
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            WeaponItem = characterStream
                .Select(c =>
                {
                    if (c == null || !c.Is(out IEquipmentHost h)) return Observable.Return<ItemId?>(null);
                    return (Observable<ItemId?>)h.Weapon;
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            inventoryViewModel.ItemSelected
                .Subscribe(EquipToFirstAvailableSlot)
                .AddTo(disposables);
        }

        public void RequestEquip(EquipmentSlot slot, ItemId itemId)
        {
            equipmentHost?.Equip(slot, itemId);
        }

        public void RequestUnequip(EquipmentSlot slot)
        {
            equipmentHost?.Unequip(slot);
        }

        void EquipToFirstAvailableSlot(ItemId itemId)
        {
            if (equipmentHost == null) return;

            if (!equipmentHost.Head.Value.HasValue)
                equipmentHost.Equip(EquipmentSlot.Head, itemId);
            else if (!equipmentHost.Body.Value.HasValue)
                equipmentHost.Equip(EquipmentSlot.Body, itemId);
            else if (!equipmentHost.Weapon.Value.HasValue)
                equipmentHost.Equip(EquipmentSlot.Weapon, itemId);
            else
                equipmentHost.Equip(EquipmentSlot.Head, itemId);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
