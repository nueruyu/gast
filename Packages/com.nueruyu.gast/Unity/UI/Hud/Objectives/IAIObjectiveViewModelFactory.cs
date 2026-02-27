using Gast.Domain.AI;

namespace Gast.Unity.UI.Hud.Objectives
{
    public interface IAIObjectiveViewModelFactory
    {
        IAIObjectiveViewModel Create(IAIObjective objective);
    }
}
