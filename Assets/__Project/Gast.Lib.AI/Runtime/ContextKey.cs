using System;

namespace Gast.Lib.AI
{
    public readonly struct ContextKey : IEquatable<ContextKey>
    {
        public object ActorId { get; }
        public string DomainName { get; }

        public ContextKey(object actorId, string domainName)
        {
            ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            DomainName = domainName ?? throw new ArgumentNullException(nameof(domainName));
        }

        public bool Equals(ContextKey other)
        {
            return Equals(ActorId, other.ActorId) && DomainName == other.DomainName;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActorId, DomainName);
        }

        public override string ToString()
        {
            return $"{ActorId}:{DomainName}";
        }

        public static bool operator ==(ContextKey left, ContextKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContextKey left, ContextKey right)
        {
            return !left.Equals(right);
        }
    }
}