// Assets/__Project/Gast/UI/Runtime/Hud/PlayerStatusViewModel.cs
using System;
using Gast.Domain.Characters;
using Gast.Domain.Players;
using Gast.Shared.Observables;
using R3;
using UnityEngine;

namespace Gast.UI.Hud
{
    public class PlayerStatusViewModel : IDisposable
    {
        private readonly CompositeDisposable disposables = new();

        public ReadOnlyReactiveProperty<float> HpRatio { get; }
        public ReadOnlyReactiveProperty<string> HpText { get; }
        public ReadOnlyReactiveProperty<int> CurrentMoney { get; }

        public PlayerStatusViewModel(IPlayerManager playerManager)
        {
            var currentCharacter = playerManager.CurrentCharacter.ToObservable();

            HpRatio = currentCharacter
                .Select(character =>
                {
                    if (character == null) return Observable.Return(0f);
                    if (!character.TryResolve(out IHasHealthStatus healthStatus)) return Observable.Return(0f);

                    return healthStatus.Health.ToObservable()
                        .CombineLatest(healthStatus.MaxHealth.ToObservable(),
                            (health, maxHealth) => maxHealth > 0 ? Mathf.Clamp01(health / maxHealth) : 0f);
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            HpText = currentCharacter
                .Select(character =>
                {
                    if (character == null) return Observable.Return(string.Empty);
                    if (!character.TryResolve(out IHasHealthStatus healthStatus)) return Observable.Return(string.Empty);

                    return healthStatus.Health.ToObservable()
                        .CombineLatest(healthStatus.MaxHealth.ToObservable(),
                            (health, maxHealth) => $"{Mathf.CeilToInt(health)} / {maxHealth}");
                })
                .Switch()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            CurrentMoney = currentCharacter
                .Select(character => character?.Wallet?.Amount.ToObservable() ?? Observable.Return(0))
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
