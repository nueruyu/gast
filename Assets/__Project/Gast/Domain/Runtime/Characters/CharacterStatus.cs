using Gast.Core.Observables;
using Gast.Domain.Stats;
using System.Collections.Generic;

namespace Gast.Domain.Characters
{
    public class CharacterStatus : IStatRegistrar
    {
        readonly Dictionary<StatId, object> stats = new();

        public CharacterStatus(IStatSchema schema)
        {
            schema.Initialize(this);
        }

        void IStatRegistrar.Register<T>(IStatDefinition<T> definition, T initialValue)
        {
            stats[definition.Id] = new Live<T>(initialValue);
        }

        public ILive<T> GetStat<T>(StatId statId)
        {
            if (!TryGetStat<T>(statId, out var stat))
            {
                throw new KeyNotFoundException($"Stat '{statId}' of type '{typeof(T).Name}' not found in character status.");
            }
            return stat;
        }

        public bool TryGetStat<T>(StatId statId, out ILive<T> stat)
        {
            if (stats.TryGetValue(statId, out var liveObject) && liveObject is ILive<T> liveStat)
            {
                stat = liveStat;
                return true;
            }
            stat = null;
            return false;
        }

        public bool TryGetStatValue<T>(StatId statId, out T value)
        {
            if (TryGetStat<T>(statId, out var stat))
            {
                value = stat.Value;
                return true;
            }
            value = default;
            return false;
        }

        public void SetStat<T>(StatId statId, T value)
        {
            if (stats.TryGetValue(statId, out var liveObject) && liveObject is Live<T> liveStat)
            {
                liveStat.Value = value;
            }
            else
            {
                stats[statId] = new Live<T>(value);
            }
        }

        public IReadOnlyDictionary<StatId, object> GetAllStats() => stats;
    }
}