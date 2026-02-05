using Gast.Core.Observables;

namespace Gast.Domain.AI
{
    public interface IAIObjective
    {
        ILive<bool> IsCompleted { get; }
    }
}
