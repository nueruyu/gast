using System.Collections.Generic;

namespace Gast.Lib.AI
{
    public class PlanningStateStore
    {
        readonly Dictionary<object, object> state = new();

        public void Set(object key, object value)
        {
            state[key] = value;
        }

        public bool TryGet<T>(object key, out T value)
        {
            if (state.TryGetValue(key, out var objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        public void CopyFrom(PlanningStateStore source)
        {
            state.Clear();
            foreach (var pair in source.state)
                state[pair.Key] = pair.Value;
        }

        internal void Clear()
        {
            state.Clear();
        }
    }
}