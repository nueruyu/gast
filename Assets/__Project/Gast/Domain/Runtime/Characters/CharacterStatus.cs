using Gast.Core.Observables;
using Gast.Domain.Stats;
using System.Collections.Generic;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Manages dynamic character stats.
    /// Uses a dictionary of observable Live values for reactive state updates.
    /// This is a generic container and holds no logic about specific stats like Health.
    /// </summary>
    public class CharacterStatus
    {
        readonly Dictionary<StatId, Live<float>> stats = new();

        public CharacterStatus(IEnumerable<(IStatDefinition def, float value)> initialStats)
        {
            foreach (var (def, value) in initialStats)
            {
                stats[def.Id] = new Live<float>(value);
            }
        }

        public ILive<float> GetStat(StatId statId)
        {
            if (!TryGetStat(statId, out var stat))
            {
                throw new KeyNotFoundException($"Stat '{statId}' not found in character status.");
            }

            return stat;
        }

        /// <summary>
        /// Tries to get the observable Live object for a stat.
        /// </summary>
        public bool TryGetStat(StatId statId, out ILive<float> stat)
        {
            if (stats.TryGetValue(statId, out var liveStat))
            {
                stat = liveStat;
                return true;
            }
            stat = null;
            return false;
        }

        /// <summary>
        /// Tries to get the current value of a stat.
        /// </summary>
        public bool TryGetStatValue(StatId statId, out float value)
        {
            if (stats.TryGetValue(statId, out var liveStat))
            {
                value = liveStat.Value;
                return true;
            }
            value = 0;
            return false;
        }

        /// <summary>
        /// Sets the value of a stat. If the stat doesn't exist, it will be added.
        /// </summary>
        public void SetStat(StatId statId, float value)
        {
            if (stats.TryGetValue(statId, out var liveStat))
            {
                liveStat.Value = value;
            }
            else
            {
                stats[statId] = new Live<float>(value);
            }
        }

        /// <summary>
        /// Gets all stats as a read-only dictionary.
        /// </summary>
        public IReadOnlyDictionary<StatId, Live<float>> GetAllStats() => stats;
    }
}