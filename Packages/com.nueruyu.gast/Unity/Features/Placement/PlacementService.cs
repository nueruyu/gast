using Gast.Core.Observables;
using Gast.Domain.Economy;
using Gast.Domain.Inputs;

namespace Gast.Unity.Features.Placement
{
    public class PlacementService
    {
        readonly IInputModeManager inputModeManager;

        readonly Live<bool> isActive = new(false);
        readonly Live<ItemId> currentItemId = new(default);

        public ILive<bool> IsActive => isActive;
        public ILive<ItemId> CurrentItemId => currentItemId;

        public PlacementService(IInputModeManager inputModeManager)
        {
            this.inputModeManager = inputModeManager;
        }

        public void EnterMode(ItemId itemId)
        {
            currentItemId.Value = itemId;
            isActive.Value = true;
            inputModeManager.SetMode(InputMode.Placement);
        }

        public void ExitMode()
        {
            isActive.Value = false;
            currentItemId.Value = default;
            inputModeManager.SetMode(InputMode.Gameplay);
        }
    }
}
