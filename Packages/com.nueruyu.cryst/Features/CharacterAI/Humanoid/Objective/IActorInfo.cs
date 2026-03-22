using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public interface IActorInfo
    {
        CharacterId Id { get; }
        CharacterTypeId TypeId { get; }
        Vector3 Position { get; }
        Faction Faction { get; }
        bool IsAlive { get; }
    }
}
