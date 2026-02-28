using R3;
using UnityEngine;

namespace Gast.Unity.UI.Hud.Objectives
{
    public interface IAIObjectiveViewModel
    {
        ReadOnlyReactiveProperty<bool> IsCompleted { get; }
        ReadOnlyReactiveProperty<float> ProgressRatio { get; }
        ReadOnlyReactiveProperty<string> ProgressText { get; }
        string Description { get; }
        Sprite ItemIcon { get; }
    }
}
