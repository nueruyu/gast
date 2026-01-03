using Gast.Domain.Inputs;
using R3;
using System;

namespace Gast.UI.Interactions
{
    public class MenuViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();
        readonly ReactiveProperty<bool> visible = new(false);

        public ReadOnlyReactiveProperty<bool> Visible { get; }

        public MenuViewModel(
            IInputModeManager inputModeManager,
            IInputProvider inputProvider)
        {
            inputProvider.ShowMenu
                .Subscribe(() =>
                {
                    visible.Value = true;
                })
                .AddTo(disposables);

            inputProvider.HideMenu
                .Subscribe(() =>
                {
                    visible.Value = false;
                })
                .AddTo(disposables);

            Visible = visible
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}