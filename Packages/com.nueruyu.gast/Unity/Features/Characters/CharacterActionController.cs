using System;
using System.Collections.Generic;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    public class CharacterActionController : ICharacterActionController
    {
        readonly Dictionary<Type, ICharacterExecutableAction> registeredActions = new();
        ICharacterAction defaultAction;
        ICharacterExecutableAction activeAction;

        public ICharacterAction CurrentAction => activeAction ?? defaultAction;

        public void RegisterDefaultAction(ICharacterAction action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (defaultAction is not null)
                throw new InvalidOperationException();

            defaultAction = action;
        }

        public void RegisterAction(ICharacterExecutableAction action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            registeredActions[action.CommandType] = action;
        }

        public void ExecuteAction<TCommand>(in TCommand command) where TCommand : struct, ICharacterTriggerCommand
        {
            TryExecute(in command);
        }

        public void StartAction<TCommand>(in TCommand command) where TCommand : struct, ICharacterStateCommand
        {
            TryExecute(in command);
        }

        public bool TryExecute<TCommand>(in TCommand command)
            where TCommand : struct, ICharacterActionCommand
        {
            if (!TryGetAction<TCommand>(out var action))
                return false;

            if (!action.CanExecute())
                return false;

            var active = activeAction ?? defaultAction;
            if (active != null)
            {
                if (action.Priority <= active.Priority)
                    return false;

                active.OnEnd();
            }

            activeAction = action;
            action.Execute(in command);

            return true;
        }

        public void StopAction<TCommand>() where TCommand : struct, ICharacterStateCommand
        {
            if (activeAction?.CommandType == typeof(TCommand))
            {
                CancelCurrent();
            }
        }

        public void CancelCurrent()
        {
            if (activeAction != null)
            {
                activeAction.OnEnd();
                activeAction = null;
            }
        }

        public void Move(Vector3 direction)
        {
            CurrentAction?.Move(direction);
        }

        public void Update()
        {
            if (activeAction != null)
            {
                if (!activeAction.OnUpdate())
                {
                    activeAction.OnEnd();
                    activeAction = null;
                }
            }
        }

        public bool IsActionActive<TCommand>() where TCommand : struct, ICharacterActionCommand
        {
            return activeAction != null && activeAction.CommandType == typeof(TCommand);
        }

        public bool CanExecuteAction<TCommand>() where TCommand : struct, ICharacterActionCommand
        {
            return TryGetAction<TCommand>(out var action) && action.CanExecute();
        }

        public bool TryGetAction<TCommand>(out ICharacterExecutableAction<TCommand> action)
            where TCommand : struct, ICharacterActionCommand
        {
            action = default;

            if (!registeredActions.TryGetValue(typeof(TCommand), out var rawAction))
                return false;

            if (rawAction is not ICharacterExecutableAction<TCommand> typedAction)
                return false;

            action = typedAction;
            return true;
        }
    }
}
