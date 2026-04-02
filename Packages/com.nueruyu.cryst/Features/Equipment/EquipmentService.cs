using System;
using System.Threading;
using System.Threading.Tasks;
using Cryst.Domain.Characters;
using Cryst.Features.Economy.Effects;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Domain.Players;
using Gast.Unity.Shared.Observables;
using R3;

namespace Cryst.Features.Equipment
{
    public class EquipmentService : ILifecycleTask, IDisposable
    {
        readonly IPlayerManager playerManager;
        readonly IItemRepository itemRepository;
        readonly CompositeDisposable disposables = new();

        IDisposable characterDisposable;

        public EquipmentService(IPlayerManager playerManager, IItemRepository itemRepository)
        {
            this.playerManager = playerManager;
            this.itemRepository = itemRepository;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            playerManager.CurrentCharacter
                .ToObservable()
                .Subscribe(OnCharacterChanged)
                .AddTo(disposables);

            return Task.CompletedTask;
        }

        void OnCharacterChanged(ICharacter character)
        {
            characterDisposable?.Dispose();
            characterDisposable = null;

            if (character == null) return;
            if (!character.Is(out IEquipmentHost equipmentHost)) return;
            if (!character.Is(out BaseCharacter baseCharacter)) return;

            var status = baseCharacter.Status;
            var d = new CompositeDisposable();

            Observable.Merge(
                equipmentHost.Head.AsUnitObservable(),
                equipmentHost.Body.AsUnitObservable(),
                equipmentHost.Weapon.AsUnitObservable()
            )
            .Subscribe(_ => RecalculateStats(equipmentHost, status))
            .AddTo(d);

            characterDisposable = d;
        }

        void RecalculateStats(IEquipmentHost equipmentHost, CharacterStatus status)
        {
            var builder = new StatBuilder();
            ApplyEffectIfEquipped(equipmentHost.Head.Value, builder);
            ApplyEffectIfEquipped(equipmentHost.Body.Value, builder);
            ApplyEffectIfEquipped(equipmentHost.Weapon.Value, builder);
            status.ApplyEquipmentBonuses(builder);
        }

        void ApplyEffectIfEquipped(ItemId? itemId, StatBuilder builder)
        {
            if (!itemId.HasValue) return;

            var definition = itemRepository.Get(itemId.Value);
            foreach (var effect in definition.Effects)
            {
                if (effect is EquipmentEffect equipmentEffect)
                    equipmentEffect.ApplyTo(builder);
            }
        }

        public void Dispose()
        {
            characterDisposable?.Dispose();
            disposables.Dispose();
        }
    }
}
