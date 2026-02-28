using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Application.Characters
{
    [Serializable]
    public readonly struct CreatePlayerCommand : ICommand<ICharacter>
    {
        public ICharacterCreationParameters Parameters { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public CreatePlayerCommand(ICharacterCreationParameters parameters, Vector3 position, Quaternion rotation)
        {
            Parameters = parameters;
            Position = position;
            Rotation = rotation;
        }
    }
}
