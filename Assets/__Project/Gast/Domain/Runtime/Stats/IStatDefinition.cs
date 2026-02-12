using System;

namespace Gast.Domain.Stats
{
    public interface IStatDefinition
    {
        StatId Id { get; }
        string DisplayName { get; }
        Type ValueType { get; }
    }

    public interface IStatDefinition<T> : IStatDefinition
    {
    }
}