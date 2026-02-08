using Gast.Domain.Players;
using Gast.Domain.Stats;
using Gast.Shared.Observables;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.UI.Hud
{
    /// <summary>
    /// ViewModel for the game HUD that processes character status data for UI presentation.
    /// </summary>
    public class GameHudViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();
        readonly ReactiveProperty<bool> hasFocus = new(false);
        readonly IPlayerManager playerManager;

        public ReadOnlyReactiveProperty<float> HpRatio { get; }
        public ReadOnlyReactiveProperty<string> HpText { get; }
        public ReadOnlyReactiveProperty<bool> IsVisible { get; }
        public ReadOnlyReactiveProperty<int> CurrentMoney { get; }
        public ReadOnlyReactiveProperty<IReadOnlyList<ItemStackViewModel>> InventoryItems { get; }
        public ReadOnlyReactiveProperty<bool> HasFocus => hasFocus;

        public ReadOnlyReactiveProperty<bool> IsAiControlActive { get; }
        public ReadOnlyReactiveProperty<IReadOnlyList<IAIObjectiveViewModel>> AiObjectives { get; }

        public GameHudViewModel(
            IPlayerManager playerManager,
            ItemStackViewModelFactory itemStackViewModelFactory,
            AIObjectiveViewModelFactory objectiveViewModelFactory)
        {
            this.playerManager = playerManager;

            var healthStatId = StatId.FromString("Health");
            var maxHealthStatId = StatId.FromString("MaxHealth");

            // Added: Monitor if an AI Brain is currently active
            IsAiControlActive = playerManager.CurrentAIBrain
                .ToObservable()
                .Select(brain => brain != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            AiObjectives = playerManager.CurrentAIBrain.ToObservable()
                .Select(brain =>
                {
                    if (brain == null)
                    {
                        return (IReadOnlyList<IAIObjectiveViewModel>)Array.Empty<IAIObjectiveViewModel>();
                    }
                    return brain.CurrentObjectives
                        .Select(objectiveViewModelFactory.Create)
                        .ToList();
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            var currentCharacter = playerManager.CurrentCharacter
                .ToObservable();

            IsVisible = currentCharacter
                .Select(character => character != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HpRatio = currentCharacter
                .Select(character =>
                {
                    if (character == null)
                        return Observable.Return(0f);

                    if (!character.Status.TryGetStat(healthStatId, out var health))
                        return Observable.Return(0f);

                    character.Status.TryGetStatValue(maxHealthStatId, out var maxHealth);
                    return health.ToObservable()
                        .Select(current => maxHealth > 0 ? Mathf.Clamp01(current / maxHealth) : 0f);
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HpText = currentCharacter
                .Select(character =>
                {
                    if (character == null)
                        return Observable.Return(string.Empty);

                    if (!character.Status.TryGetStat(healthStatId, out var health))
                        return Observable.Return(string.Empty);

                    character.Status.TryGetStatValue(maxHealthStatId, out var maxHealth);
                    return health.ToObservable()
                        .Select(current => $"{Mathf.CeilToInt(current)} / {maxHealth}");
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            CurrentMoney = currentCharacter
                .Select(character =>
                {
                    if (character == null)
                        return Observable.Return(0);

                    var wallet = character.Wallet;
                    return wallet?.Amount.ToObservable() ?? Observable.Return(0);
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            InventoryItems = currentCharacter
                .Select(character =>
                {
                    if (character == null)
                        return Observable.Return((IReadOnlyList<ItemStackViewModel>)Array.Empty<ItemStackViewModel>());

                    var inventory = character.Inventory;

                    return character.Inventory.InventoryChanged
                        .ToObservable()
                        .Prepend(Unit.Default)
                        .Select(_ => (IReadOnlyList<ItemStackViewModel>)inventory.Items
                            .Select(itemStackViewModelFactory.Create)
                            .ToArray());
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void StopAiControl()
        {
            playerManager.RestorePlayerControl();
        }

        public void SetFocus(bool hasFocus)
        {
            this.hasFocus.Value = hasFocus;
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}