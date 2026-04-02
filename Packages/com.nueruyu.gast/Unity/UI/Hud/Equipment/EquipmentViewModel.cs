using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Application.Equipment;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Domain.Players;
using Gast.Unity.Features.Equipment;
using Gast.Unity.Shared.Observables;
using R3;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();
        readonly IPlayerManager playerManager;
        readonly UnequipItemUseCase unequipUseCase;

        public IReadOnlyList<EquipmentSlotViewModel> SlotViewModels { get; }

        public EquipmentViewModel(
            IPlayerManager playerManager,
            IEquipmentSlotProvider slotProvider,
            UnequipItemUseCase unequipUseCase)
        {
            this.playerManager = playerManager;
            this.unequipUseCase = unequipUseCase;

            var characterStream = playerManager.CurrentCharacter.ToObservable();

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

                return new EquipmentSlotViewModel(def, item, RequestUnequip);
            }).ToArray();
        }

        void RequestUnequip(EquipmentSlotId slotId)
        {
            var character = playerManager.CurrentCharacter.Value;
            if (character == null) return;
            unequipUseCase.Execute(new UnequipItemCommand(character.Id, slotId));
        }

        public void Dispose() => disposables.Dispose();
    }
}
