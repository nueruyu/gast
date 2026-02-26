using Gast.Domain.AI;

namespace Gast.UI.Hud.Objectives
{
    public interface IAIObjectiveViewModelFactory
    {
        IAIObjectiveViewModel Create(IAIObjective objective);
    }
}
