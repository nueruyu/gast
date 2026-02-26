using Gast.Core.Observables;
using Gast.Domain.Inputs;

namespace Gast.Features.Inputs
{
    public class InputModeManager : IInputModeManager
    {
        readonly Live<InputMode> currentMode = new(InputMode.UI);

        public ILive<InputMode> CurrentMode => currentMode;

        public void SetMode(InputMode mode)
        {
            currentMode.Value = mode;
        }
    }
}