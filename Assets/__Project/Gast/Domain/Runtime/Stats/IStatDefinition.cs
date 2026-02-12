using System;

namespace Gast.Domain.Stats
{
    public interface IStatDefinition
    {
        StatId Id { get; }
        string DisplayName { get; }
        Type ValueType { get; }

        object GetDefaultValueAsObject();

        object ParseValue(string value);
    }

    public interface IStatDefinition<T> : IStatDefinition
    {
        T DefaultValue { get; }
    }
}