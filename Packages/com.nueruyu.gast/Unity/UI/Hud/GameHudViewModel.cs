using Gast.Domain.Players;
using Gast.Shared.Observables;
using R3;
using System;
using System.Collections.Generic;

namespace Gast.UI.Hud
{
    public class GameHudViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();
        readonly ReactiveProperty<bool> hasFocus = new(false);

        public ReadOnlyReactiveProperty<bool> IsVisible { get; }
        public ReadOnlyReactiveProperty<bool> HasFocus => hasFocus;

        public GameHudViewModel(IPlayerManager playerManager)
        {
            IsVisible = playerManager.CurrentCharacter
                .ToObservable()
                .Select(character => character != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
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
