// Assets/__Project/Gast/UI/Runtime/Hud/AIStatusViewModel.cs
using System;
using Gast.Domain.Players;
using Gast.Shared.Observables;
using R3;

namespace Gast.UI.Hud
{
    public class AIStatusViewModel : IDisposable
    {
        private readonly CompositeDisposable disposables = new();
        private readonly IPlayerManager playerManager;

        public ReadOnlyReactiveProperty<bool> IsAiControlActive { get; }

        public AIStatusViewModel(IPlayerManager playerManager)
        {
            this.playerManager = playerManager;

            IsAiControlActive = playerManager.CurrentAIBrain
                .ToObservable()
                .Select(brain => brain != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void StopAiControl()
        {
            playerManager.RestorePlayerControl();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
