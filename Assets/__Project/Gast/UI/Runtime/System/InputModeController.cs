using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.UI.Hud;
using Gast.UI.Menu;
using R3;
using System.Threading;
using System.Threading.Tasks;

namespace Gast.UI.System
{
    public class InputModeController : ILifecycleTask
    {
        readonly IInputModeManager inputModeManager;
        readonly GameHudViewModel gameHudViewModel;
        readonly MenuViewModel menuViewModel;

        public InputModeController(
            IInputModeManager inputModeManager,
            GameHudViewModel gameHudViewModel,
            MenuViewModel menuViewModel)
        {
            this.inputModeManager = inputModeManager;
            this.gameHudViewModel = gameHudViewModel;
            this.menuViewModel = menuViewModel;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            menuViewModel.Visible.CombineLatest(
                gameHudViewModel.HasFocus,
                (menuVisible, hudHasFocus) => (menuVisible, hudHasFocus))
                .Subscribe(context =>
                {
                    if (context.hudHasFocus && !context.menuVisible)
                    {
                        inputModeManager.SetMode(InputMode.Gameplay);
                    }
                    else
                    {
                        inputModeManager.SetMode(InputMode.UI);
                    }
                }).AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}