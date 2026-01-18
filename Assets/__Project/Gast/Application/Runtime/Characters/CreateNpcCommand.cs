using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Application.Characters
{
    [Serializable]
    public readonly struct CreateNpcCommand : ICommand<ICharacter>
    {
        public CharacterTypeId TypeId { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Faction Faction { get; }

        public CreateNpcCommand(CharacterTypeId typeId, Vector3 position, Quaternion rotation, Faction faction)
        {
            TypeId = typeId;
            Position = position;
            Rotation = rotation;
            Faction = faction;
        }
    }
}