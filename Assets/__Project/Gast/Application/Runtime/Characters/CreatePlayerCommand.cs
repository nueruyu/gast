using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Api.Characters
{
    [Serializable]
    public readonly struct CreatePlayerCommand : ICommand<ICharacter>
    {
        public CharacterTypeId TypeId { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public CreatePlayerCommand(CharacterTypeId typeId, Vector3 position, Quaternion rotation)
        {
            TypeId = typeId;
            Position = position;
            Rotation = rotation;
        }
    }
}