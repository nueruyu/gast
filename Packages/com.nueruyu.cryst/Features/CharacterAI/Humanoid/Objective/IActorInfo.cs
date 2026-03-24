using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public readonly struct ActorInfo : IActorInfo
    {
        public CharacterId Id { get; }
        public CharacterTypeId TypeId { get; }
        public Vector3 Position { get; }
        public Faction Faction { get; }
        public bool IsAlive { get; }

        public ActorInfo(CharacterId id, CharacterTypeId typeId, Vector3 position, Faction faction, bool isAlive)
        {
            Id = id;
            TypeId = typeId;
            Position = position;
            Faction = faction;
            IsAlive = isAlive;
        }
    }

    public interface IActorInfo
    {
        CharacterId Id { get; }
        CharacterTypeId TypeId { get; }
        Vector3 Position { get; }
        Faction Faction { get; }
        bool IsAlive { get; }
    }
}
