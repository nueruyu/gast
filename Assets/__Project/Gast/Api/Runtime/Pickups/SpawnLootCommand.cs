using Gast.Core.Commands;
using Gast.Domain.Characters;
using System;
using UnityEngine;

namespace Gast.Api.Pickups
{
    [Serializable]
    public readonly struct SpawnLootCommand : ICommand
    {
        public CharacterTypeId CharacterTypeId { get; }
        public Vector3 Position { get; }

        public SpawnLootCommand(CharacterTypeId characterTypeId, Vector3 position)
        {
            CharacterTypeId = characterTypeId;
            Position = position;
        }
    }
}