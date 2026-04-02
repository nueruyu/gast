using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gast.Core.Stats;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Unity.Shared.Observables;
using R3;

namespace Gast.Unity.Features.Equipment
{
    public class EquipmentService : ILifecycleTask, IDisposable
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;
        readonly CompositeDisposable disposables = new();
        readonly Dictionary<CharacterId, IDisposable> characterSubscriptions = new();

        public EquipmentService(ICharacterRepository characterRepository, IItemRepository itemRepository)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var character in characterRepository.GetAll())
                SubscribeCharacter(character);

            characterRepository.Registered.ToObservable()
                .Subscribe(SubscribeCharacter)
                .AddTo(disposables);

            characterRepository.Unregistered.ToObservable()
                .Subscribe(character =>
                {
                    if (characterSubscriptions.Remove(character.Id, out var d))
                        d.Dispose();
                })
                .AddTo(disposables);

            return Task.CompletedTask;
        }

        void SubscribeCharacter(ICharacter character)
        {
            if (!character.Is(out IEquipmentHost equipmentHost)) return;

            character.Is(out IEquipmentBonusApplicable bonusApplicable);

            var slotObservables = equipmentHost.Slots
                .Select(s => equipmentHost.GetSlot(s).AsUnitObservable())
                .ToArray();

            characterSubscriptions[character.Id] = Observable
                .Merge(slotObservables)
                .Subscribe(_ => RecalculateStats(equipmentHost, bonusApplicable));
        }

        void RecalculateStats(IEquipmentHost equipmentHost, IEquipmentBonusApplicable bonusApplicable)
        {
            var builder = new StatBuilder();
            foreach (var slotId in equipmentHost.Slots)
                ApplyEffectIfEquipped(equipmentHost.GetSlot(slotId).Value, builder);

            bonusApplicable?.ApplyEquipmentBonuses(builder);
        }

        void ApplyEffectIfEquipped(ItemId? itemId, StatBuilder builder)
        {
            if (!itemId.HasValue) return;

            var definition = itemRepository.Get(itemId.Value);
            foreach (var effect in definition.EquipmentEffects)
                effect.ApplyTo(builder);
        }

        public void Dispose()
        {
            foreach (var d in characterSubscriptions.Values)
                d.Dispose();
            disposables.Dispose();
        }
    }
}
