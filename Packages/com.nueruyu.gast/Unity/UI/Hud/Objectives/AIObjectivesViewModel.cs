using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Players;
using Gast.Unity.Shared.Observables;
using R3;

namespace Gast.Unity.UI.Hud.Objectives
{
    public class AIObjectivesViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();

        public ReadOnlyReactiveProperty<bool> IsAiControlActive { get; }
        public ReadOnlyReactiveProperty<IReadOnlyList<IAIObjectiveViewModel>> AiObjectives { get; }

        public AIObjectivesViewModel(IPlayerManager playerManager, IAIObjectiveViewModelFactory objectiveViewModelFactory)
        {
            IsAiControlActive = playerManager.CurrentAIBrain.ToObservable()
                .Select(brain => brain != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            AiObjectives = playerManager.CurrentAIBrain.ToObservable()
                .Select(brain =>
                {
                    if (brain == null)
                    {
                        return (IReadOnlyList<IAIObjectiveViewModel>)Array.Empty<IAIObjectiveViewModel>();
                    }
                    return brain.CurrentObjectives
                        .Select(objectiveViewModelFactory.Create)
                        .ToList();
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
