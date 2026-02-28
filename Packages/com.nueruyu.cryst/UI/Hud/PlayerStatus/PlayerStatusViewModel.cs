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
        public ReadOnlyReactiveProperty<int> CurrentMoney { get; }

        public PlayerStatusViewModel(IPlayerManager playerManager)
        {
            var currentCharacter = playerManager.CurrentCharacter
                .ToObservable()
                .Where(x => x != null);

            var healthInfo = currentCharacter
                .Select(character =>
                {
                    var status = character.As<BaseCharacter>().Status;
                    return status.Health.ToObservable()
                        .CombineLatest(status.MaxHealth.ToObservable(),
                            (health, maxHealth) => (health, maxHealth));
                })
                .Switch()
                .Share();

            HpRatio = healthInfo
                .Select(info =>
                {
                    var (health, maxHealth) = info;
                    return maxHealth > 0 ? Mathf.Clamp01(health / maxHealth) : 0f;
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HpText = healthInfo
                .Select(info =>
                {
                    var (health, maxHealth) = info;
                    return $"{Mathf.CeilToInt(health)} / {maxHealth}";
                })
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