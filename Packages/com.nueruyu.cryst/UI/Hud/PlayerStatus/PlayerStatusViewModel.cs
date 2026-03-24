using System;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Players;
using Gast.Unity.Shared.Observables;
using R3;
using UnityEngine;

namespace Cryst.UI.Hud.PlayerStatus
{
    public class PlayerStatusViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();

        public ReadOnlyReactiveProperty<float> HpRatio { get; }
        public ReadOnlyReactiveProperty<string> HpText { get; }
        public ReadOnlyReactiveProperty<float> HungerRatio { get; }
        public ReadOnlyReactiveProperty<string> HungerText { get; }
        public ReadOnlyReactiveProperty<int> CurrentMoney { get; }

        public PlayerStatusViewModel(IPlayerManager playerManager)
        {
            var currentCharacter = playerManager.CurrentCharacter
                .ToObservable()
                .Where(x => x != null);

            var statusInfo = currentCharacter
                .Select(character =>
                {
                    var status = character.As<BaseCharacter>().Status;
                    return status.Health.ToObservable()
                        .CombineLatest(status.MaxHealth.ToObservable(),
                            status.Hunger.ToObservable(),
                            status.MaxHunger.ToObservable(),
                            (h, mh, hu, mhu) => new { Health = h, MaxHealth = mh, Hunger = hu, MaxHunger = mhu });
                })
                .Switch()
                .Share();

            HpRatio = statusInfo
                .Select(info => info.MaxHealth > 0 ? Mathf.Clamp01(info.Health / info.MaxHealth) : 0f)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HpText = statusInfo
                .Select(info => $"{Mathf.CeilToInt(info.Health)} / {info.MaxHealth}")
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HungerRatio = statusInfo
                .Select(info => info.MaxHunger > 0 ? Mathf.Clamp01(info.Hunger / info.MaxHunger) : 0f)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HungerText = statusInfo
                .Select(info => $"{Mathf.CeilToInt(info.Hunger)} / {info.MaxHunger}")
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            CurrentMoney = currentCharacter
                .Select(character =>
                {
                    var walletHost = character.As<IWalletHost>();
                    return walletHost.Wallet.Amount.ToObservable();
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