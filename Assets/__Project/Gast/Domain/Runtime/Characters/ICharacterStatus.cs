using Gast.Core.Observables;
using Gast.Domain.Stats;
using System.Collections.Generic;

namespace Gast.Domain.Characters
{
    public interface ICharacterStatus
    {
        IReadOnlyDictionary<StatId, object> GetAllStats();
        ILive<T> GetStat<T>(StatId statId);
        void SetStat<T>(StatId statId, T value);
        bool TryGetStat<T>(StatId statId, out ILive<T> stat);
        bool TryGetStatValue<T>(StatId statId, out T value);
    }
}