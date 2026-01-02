using Cysharp.Threading.Tasks;
using DescrioGames.Core.Observables;
using DescrioGames.Core.Tasks;
using DescrioGames.Domain.Cameras;
using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Interactions;
using DescrioGames.Domain.Players;
using DescrioGames.Features.Players;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DescrioGames.Features.Gameplay
{
    /// <summary>
    /// Binds player character changes to dependent systems (camera, interactions).
    /// Automatically updates camera follow target and interaction detector origin
    /// when the player possesses a new character.
    /// </summary>
    public class PlayerContextBinder : ILifecycleTask
    {
        readonly IPlayerManager playerManager;
        readonly ICameraService cameraService;
        readonly IInteractionSystem interactionSystem;

        public PlayerContextBinder(
            IPlayerManager playerManager,
            ICameraService cameraService,
            IInteractionSystem interactionSystem)
        {
            this.playerManager = playerManager;
            this.cameraService = cameraService;
            this.interactionSystem = interactionSystem;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            playerManager.CurrentCharacter
                .SubscribeWithCurrent(OnPlayerCharacterChanged)
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnPlayerCharacterChanged(ICharacter character)
        {
            if (character == null)
            {
                cameraService.UnsetFollowTarget();
                interactionSystem.UnsetInteractor();

                Debug.Log("PlayerContextBinder: Player unpossessed, cleared camera and interaction targets");
            }
            else
            {
                cameraService.SetFollowTarget(character.Id);
                interactionSystem.SetInteractor(character.Id);

                Debug.Log($"PlayerContextBinder: Updated camera and interaction targets to {character.Id}");
            }
        }
    }
}