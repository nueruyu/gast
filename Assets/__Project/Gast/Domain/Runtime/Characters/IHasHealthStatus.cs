using Gast.Core.Observables;

namespace Gast.Domain.Characters
{
    public interface IHasHealthStatus
    {
        ILive<float> Health { get; }
        ILive<float> MaxHealth { get; }
    }
}