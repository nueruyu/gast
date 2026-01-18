using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using System;

namespace Gast.Application.Interactions
{
    [Serializable]
    public readonly struct InteractCommand : IAsyncCommand<bool>
    {
        public CharacterId InteractorId { get; }
        public InteractableId TargetId { get; }

        public InteractCommand(CharacterId interactorId, InteractableId targetId)
        {
            InteractorId = interactorId;
            TargetId = targetId;
        }
    }
}