using Gast.Features.Characters;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Interface for actions that can be manually stopped (state-based actions).
    /// </summary>
    public interface IStatefulCharacterAction : ICharacterAction
    {
        void Stop();
    }
}
