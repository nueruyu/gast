using Gast.Domain.Players;
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

        public ReadOnlyReactiveProperty<float> HpRatio { get; }
        public ReadOnlyReactiveProperty<string> HpText { get; }
        public ReadOnlyReactiveProperty<bool> IsVisible { get; }
        public ReadOnlyReactiveProperty<int> CurrentMoney { get; }
        public ReadOnlyReactiveProperty<IReadOnlyList<ItemStackViewModel>> InventoryItems { get; }

        public GameHudViewModel(
            IPlayerManager playerManager,
            ItemStackViewModelFactory itemStackViewModelFactory)
        {
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
                    {
                        return Observable.Return(0f);
                    }

                    var maxHealth = character.Status.MaxHealth;
                    return character.Status.Health
                        .ToObservable()
                        .Select(current => maxHealth > 0 ? Mathf.Clamp01(current / maxHealth) : 0f);
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HpText = currentCharacter
                .Select(character =>
                {
                    if (character == null)
                    {
                        return Observable.Return(string.Empty);
                    }

                    var maxHealth = character.Status.MaxHealth;
                    return character.Status.Health
                        .ToObservable()
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

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}