using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Manages character actions through an ActionRouter.
    /// Provides command-based dispatch for action execution.
    /// </summary>
    public class CharacterActionController : ICharacterActionController
    {
        readonly CharacterActionRouter router = new();
        readonly CharacterContext character;

        public CharacterActionController(CharacterContext character)
        {
            this.character = character;
        }

        public void RegisterAction(ICharacterAction action)
        {
            router.Register(action);
        }

        public void ExecuteAction<TCommand>(in TCommand command) where TCommand : struct, ICharacterTriggerCommand
        {
            router.TryExecute(in command);
        }

        public void StartAction<TCommand>(in TCommand command) where TCommand : struct, ICharacterStateCommand
        {
            router.TryExecute(in command);
        }

        public void StopAction<TCommand>() where TCommand : struct, ICharacterStateCommand
        {
            router.Stop<TCommand>();
        }

        public void Move(Vector3 direction)
        {
            router.CurrentAction?.Move(direction);
        }

        public void Update()
        {
            router.Update();
        }

        public bool IsActionActive<TCommand>() where TCommand : struct, ICharacterActionCommand
        {
            return router.IsActionActive<TCommand>();
        }

        public bool CanExecuteAction<TCommand>() where TCommand : struct, ICharacterActionCommand
        {
            return router.TryGetAction<TCommand>(out var action) && action.CanExecute();
        }
    }
}