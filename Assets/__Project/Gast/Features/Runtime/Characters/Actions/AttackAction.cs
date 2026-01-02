using UnityEngine;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Gast.Features.Combat;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Attack action that stops movement and executes the attack method.
    /// </summary>
    public class AttackAction : ICharacterAction
    {
        readonly CharacterContext character;

        readonly ICombatMethod method;
        readonly float cooldown;
        readonly float duration = 0.6f; // Fixed duration or from animation

        bool isActive;
        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public int Priority => 5;
        public bool IsActive => isActive;

        public AttackAction(
            CharacterContext character,
            ICombatMethod method,
            float cooldown)
        {
            this.character = character;
            this.method = method;
            this.cooldown = cooldown;
        }

        public bool CanExecute()
        {
            return !isActive && Time.time >= lastAttackTime + cooldown;
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