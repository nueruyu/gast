using System.Collections.Generic;

namespace Gast.Lib.AI
{
    public class PlanningContext
    {
        internal readonly Dictionary<object, object> state = new();

        internal void Set(object key, object value)
        {
            state[key] = value;
        }

        internal bool TryGet<T>(object key, out T value)
        {
            if (state.TryGetValue(key, out var objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        internal void Clear()
        {
            state.Clear();
        }
    }
}
