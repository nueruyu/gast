using Gast.Domain.AI;

namespace Gast.UI.Hud
{
    public interface IAIObjectiveViewModelFactory
    {
        IAIObjectiveViewModel Create(IAIObjective objective);
    }
}
