using Cryst.Modules.CharacterActions;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Features.Cameras;
using Gast.Features.Characters;
using Gast.Features.Navigations;
using Gast.Features.Sensors;
using Gast.Infrastructure.Characters;
using Gast.Shared.UnityExtensions;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterContextInitializer : ICharacterContextInitializer
    {
        public void Initialize(CharacterContext context)
        {
            var body = context.GameObject.RequireComponentInChildren<CharacterBody>();
            var visionSensor = context.GameObject.RequireComponentInChildren<ConeVisionSensor>();
            var navigationProvider = context.GameObject.RequireComponentInChildren<NavMeshNavigator>();
            var interactionSensor = context.GameObject.RequireComponentInChildren<IInteractionSensor>();
            var interactor = context.GameObject.RequireComponentInChildren<IInteractor>();
            var animator = context.GameObject.GetComponentInChildren<CharacterAnimator>();
            var audio = context.GameObject.RequireComponentInChildren<CharacterAudio>();
            var cameraFocusTarget = context.GameObject.RequireComponentInChildren<CameraFocusTarget>();

            var visionSettings = context.Resolve<ConeVisionSensorSettings>();
            if (visionSettings != null)
            {
                visionSensor.ViewAngle = visionSettings.ViewAngle;
                visionSensor.ViewRadius = visionSettings.ViewRadius;
                visionSensor.EyeOffset = visionSettings.EyeOffset;
            }

            var navSettings = context.Resolve<NavMeshNavigatorSettings>();
            if (navSettings != null)
            {
                navigationProvider.StoppingDistance = navSettings.StoppingDistance;
            }

            context.Register<IVisionSensor>(visionSensor);
            context.Register<INavigationProvider>(navigationProvider);
            context.Register(interactionSensor);
            context.Register(interactor);
            context.Register(body);
            context.Register(animator);
            context.Register(audio);
            context.Register(cameraFocusTarget);

            var stateStore = new CharacterActionStateStore();
            var movement = new CharacterMovement(
                animator,
                body,
                context.TypeDefinition);

            context.Register(stateStore);
            context.Register(movement);

            // Handlers
            var footstepSettings = context.Resolve<CharacterFootstepSettings>();
            if (footstepSettings != null)
            {
                new CharacterFootstepHandler(animator, audio, footstepSettings);
            }
        }
    }
}