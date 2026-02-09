using Gast.Domain.Characters;
using UnityEngine;

namespace GastGame.Actions.Commands
{
    public readonly struct DashCommand : ICharacterTriggerCommand
    {
        public Vector3 Direction { get; }

        public DashCommand(Vector3 direction)
        {
            Direction = direction;
        }
    }
}