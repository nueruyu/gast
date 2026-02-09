using System;
using UnityEngine;
using Gast.Features.Combat;
using Gast.Features.Characters.Actions.Commands;
using Cysharp.Threading.Tasks;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Attack action that stops movement and executes the attack method.
    /// </summary>
    public class AttackAction : ICharacterAction<AttackCommand>
    {
        readonly CharacterContext character;
        readonly MeleeMethod method;
        readonly AttackActionSettings settings;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;

        public AttackAction(
            CharacterContext character,
            AttackActionSettings settings)
        {
            this.character = character;
            this.settings = settings;
            method = new MeleeMethod(settings.MeleeMethodSettings, character);
            method.BindEvents(character).AddTo(character.Body.destroyCancellationToken);
        }

        public bool CanExecute()
        {
            return Time.time >= lastAttackTime + settings.Cooldown;
        }

        public void Execute(in AttackCommand command)
        {
            startTime = Time.time;
            lastAttackTime = startTime;

            method.Attack(character);
        }

        public bool OnUpdate()
        {
            return Time.time < startTime + settings.Duration;
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
        }
    }
}