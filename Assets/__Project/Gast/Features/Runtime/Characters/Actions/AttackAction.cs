using System;
using UnityEngine;
using Gast.Domain.Combat;
using Gast.Features.Combat;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Attack action that stops movement and executes the attack method.
    /// </summary>
    public class AttackAction : ICharacterAction
    {
        readonly CharacterContext character;
        readonly ICombatMethod method;
        readonly AttackActionSettings settings;
        readonly float duration = 0.6f;

        bool isActive;
        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;
        public bool IsActive => isActive;

        public AttackAction(
            CharacterContext character,
            ICombatMethod method,
            AttackActionSettings settings)
        {
            this.character = character;
            this.method = method;
            this.settings = settings;
        }

        public bool CanExecute()
        {
            return !isActive && Time.time >= lastAttackTime + settings.Cooldown;
        }

        public void Execute()
        {
            isActive = true;
            startTime = Time.time;
            lastAttackTime = startTime;

            method.Attack(character);
        }

        public void OnUpdate()
        {
            if (Time.time >= startTime + duration)
            {
                isActive = false;
            }
        }

        public void Move(Vector3 direction, float speed)
        {
            // Allow movement while attacking
            if (direction.sqrMagnitude > 0.01f)
            {
                character.Body.SetInputVelocity(direction * speed);
                character.Body.SetLookDirection(direction, 10f);
            }
        }

        public void OnEnd()
        {
            isActive = false;
        }
    }
}