using System.Collections.Generic;

namespace DescrioGames.Lib.AI
{
    public interface IMethodSelector<TWorldState> where TWorldState : struct
    {
        bool Select(
            IReadOnlyList<Method<TWorldState>> methods,
            ref TWorldState state,
            CheckOptions options,
            ISimulationContext context,
            out Method<TWorldState> selectedMethod);
    }
}