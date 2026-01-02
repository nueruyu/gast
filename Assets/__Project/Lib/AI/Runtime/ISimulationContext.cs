using System;
using System.Collections.Generic;

namespace DescrioGames.Lib.AI
{
    public interface ISimulationContext
    {
        void Set<T>(T data);

        bool TryGet<T>(out T data);

        void Remove<T>();

        void Clear();
    }

    public class SimulationContext : ISimulationContext
    {
        readonly Dictionary<Type, object> _data = new Dictionary<Type, object>();

        public void Set<T>(T data) => _data[typeof(T)] = data;

        public bool TryGet<T>(out T data)
        {
            if (_data.TryGetValue(typeof(T), out var obj) && obj is T typedData)
            {
                data = typedData;
                return true;
            }
            data = default;
            return false;
        }

        public void Remove<T>() => _data.Remove(typeof(T));

        public void Clear() => _data.Clear();
    }
}