using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace GastGame.Features.CharacterActions
{
    /// <summary>
    /// Attack action that stops movement and executes the attack method.
    /// </summary>
    public class AttackAction : ICharacterAction<AttackCommand>
    {
        readonly CharacterActionContext context;
        readonly MeleeMethod method;
        readonly AttackActionSettings settings;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;

        public AttackAction(
            CharacterActionContext context,
            AttackActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
            method = new MeleeMethod(settings.MeleeMethodSettings, context);
            method.BindEvents(context.CharacterContext).AddTo(context.CharacterContext.Body.destroyCancellationToken);
        }

        public bool CanExecute()
        {
            return Time.time >= lastAttackTime + settings.Cooldown;
        }

        public void Execute(in AttackCommand command)
        {
            startTime = Time.time;
            lastAttackTime = startTime;

            method.Attack(context);
        }

        public bool OnUpdate()
        {
            return Time.time < startTime + settings.Duration;
        }

        public void Move(Vector3 direction)
        {
            // Allow movement while attacking
            if (direction.sqrMagnitude > 0.01f)
            {
                var speed = context.CharacterContext.TypeDefinition.WalkSpeed;
                context.CharacterContext.Body.SetInputVelocity(direction * speed);
                context.CharacterContext.Body.SetLookDirection(direction, 10f);
            }
        }

        public void OnEnd()
        {
        }
    }
}
