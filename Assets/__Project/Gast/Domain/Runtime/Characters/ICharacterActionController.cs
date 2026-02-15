using UnityEngine;

namespace Gast.Domain.Characters
{
    public interface ICharacterActionController
    {
        bool CanExecuteAction<TCommand>() where TCommand : struct, ICharacterActionCommand;

        bool IsActionActive<TCommand>() where TCommand : struct, ICharacterActionCommand;

        void ExecuteAction<TCommand>(in TCommand command) where TCommand : struct, ICharacterTriggerCommand;

        void StartAction<TCommand>(in TCommand command) where TCommand : struct, ICharacterStateCommand;

        void StopAction<TCommand>() where TCommand : struct, ICharacterStateCommand;

        void Move(Vector3 direction);
    }
}