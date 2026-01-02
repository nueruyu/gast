using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Gast.Domain.Economy;
using Gast.Domain.Sensors;
using UnityEngine;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Composition root that connects a brain (decision-making) with a body (physics execution).
    /// </summary>
    [RequireComponent(typeof(CharacterBody))]
    public class Character : MonoBehaviour, ICharacter
    {
        ICharacterTypeDefinition typeDefinition;
        CharacterStatus status;
        ICharacterBrain currentBrain;

        CharacterActionController actionController;
        CharacterContext context;

        readonly Signal<ICharacter> destroyedSignal = new();
        readonly Signal<ICharacter> attackedSignal = new();

        bool isSprinting;

        public CharacterId Id => context.Id;
        public CharacterTypeId TypeId => context.TypeId;
        public CharacterStatus Status => status;
        public Wallet Wallet { get; private set; }
        public Inventory Inventory { get; private set; }
        public IVisionSensor VisionSensor => context.VisionSensor;
        public INavigationProvider NavigationProvider => context.NavigationProvider;
        public ICharacterBody Body => context.Body;
        public bool IsAlive => status.IsAlive.Value;
        public bool CanAttack => IsAlive && actionController.CanAttack;
        public bool IsGuarding => actionController.IsGuarding;
        public bool IsDashing => actionController.IsDashing;
        public ISignal<ICharacter> Destroyed => destroyedSignal;

        void Update()
        {
            actionController?.Update();
        }

        void LateUpdate()
        {
            UpdateAnimationMoveSpeed();
        }

        /// <summary>
        /// Update the animation speed parameter based on current velocity.
        /// </summary>
        void UpdateAnimationMoveSpeed()
        {
            if (context == null)
                return;

            var body = context.Body;
            var velocity = body.Velocity;
            velocity.y = 0f;
            if (!body.IsInputMovementEnabled)
            {
                velocity = Vector3.zero;
            }

            var currentSpeed = velocity.magnitude;
            var normalizedSpeed = currentSpeed / typeDefinition.SprintSpeed;
            context.Animator.SetMoveSpeed(normalizedSpeed);
        }

        /// <summary>
        /// Initialize the character with a specific type and faction.
        /// Called by CharacterFactory after instantiation.
        /// </summary>
        public void Initialize(
            CharacterContext context,
            ICharacterTypeDefinition typeDefinition,
            CharacterActionController actionController,
            CharacterStatus status,
            Wallet wallet,
            Inventory inventory)
        {
            this.context = context;
            this.typeDefinition = typeDefinition;
            this.actionController = actionController;
            this.status = status;
            Wallet = wallet;
            Inventory = inventory;
        }

        /// <summary>
        /// Attach a brain to this character, detaching any existing brain first.
        /// The brain's OnAttached method will be called after attachment.
        /// </summary>
        public void AttachBrain(ICharacterBrain newBrain)
        {
            currentBrain?.OnDetached();

            currentBrain = newBrain;
            currentBrain?.OnAttached(this);
        }

        /// <summary>
        /// Detach the current brain from this character.
        /// The brain's OnDetached method will be called.
        /// </summary>
        public void DetachBrain()
        {
            currentBrain?.OnDetached();
            currentBrain = null;
        }

        /// <summary>
        /// Move the character in the specified direction (0-1 normalized).
        /// </summary>
        public void Move(Vector3 direction)
        {
            if (!IsAlive || typeDefinition == null)
                return;

            var speed = isSprinting ? typeDefinition.SprintSpeed : typeDefinition.WalkSpeed;

            actionController.Move(direction, speed);
        }

        /// <summary>
        /// Set whether the character is sprinting.
        /// </summary>
        public void SetSprint(bool isSprinting)
        {
            this.isSprinting = isSprinting;
        }

        /// <summary>
        /// Make the character jump if grounded.
        /// </summary>
        public void Jump()
        {
            if (!IsAlive || typeDefinition == null)
                return;

            var body = context.Body;
            if (body.IsGrounded)
            {
                body.ApplyJump(typeDefinition.JumpForce);
            }
        }

        /// <summary>
        /// Execute an attack action.
        /// </summary>
        public void Attack()
        {
            if (!IsAlive)
                return;

            actionController.Attack();
            attackedSignal.Publish(this);
        }

        /// <summary>
        /// Perform a dash action in the specified direction.
        /// </summary>
        public void Dash(Vector3 direction)
        {
            if (!IsAlive || typeDefinition == null)
                return;

            actionController.Dash();
        }

        /// <summary>
        /// Set the guard state.
        /// </summary>
        public void SetGuard(bool active)
        {
            if (!IsAlive || typeDefinition == null)
                return;

            if (!typeDefinition.CanGuard)
                return;

            if (active)
                actionController.Guard();
            else
                actionController.StopGuard();
        }

        /// <summary>
        /// Apply damage and hit reaction to the character.
        /// </summary>
        public void TakeDamage(DamageInfo info)
        {
            if (!IsAlive)
                return;

            status.ApplyDamage(info.Amount);

            if (!status.IsAlive.Value)
            {
                Debug.Log($"[{Id}] Died.");
                actionController.Die();
            }
            else
            {
                actionController.TakeHit(info);
            }
        }

        void OnDestroy()
        {
            DetachBrain();
            destroyedSignal.Publish(this);
        }
    }
}