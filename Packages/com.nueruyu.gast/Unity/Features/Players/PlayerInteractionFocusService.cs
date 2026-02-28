using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Core.Tasks;
using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Players;
using UnityEngine;

namespace Gast.Unity.Features.Players
{
    public class PlayerInteractionFocusService : ILifecycleTask, IPlayerInteractionFocusService
    {
        readonly IPlayerManager playerManager;
        readonly ICameraService cameraService;
        readonly Live<IInteractable> focusedInteractable = new(null);

        public ILive<IInteractable> FocusedInteractable => focusedInteractable;

        public PlayerInteractionFocusService(IPlayerManager playerManager, ICameraService cameraService)
        {
            this.playerManager = playerManager;
            this.cameraService = cameraService;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var player = playerManager.CurrentCharacter.Value;
                if (player != null)
                {
                    UpdateFocus(player);
                }
                else
                {
                    focusedInteractable.Value = null;
                }

                await UniTask.Delay(100, cancellationToken: cancellationToken);
            }
        }

        void UpdateFocus(ICharacter player)
        {
            if (!player.Is(out IInteractor interactor))
            {
                focusedInteractable.Value = null;
                return;
            }

            var candidates = interactor.Sensor.DetectableInteractables;

            if (candidates.Count == 0)
            {
                focusedInteractable.Value = null;
                return;
            }

            if (candidates.Count == 1)
            {
                focusedInteractable.Value = candidates[0];
                return;
            }

            var mainCamera = cameraService.MainCamera;
            if (mainCamera == null)
            {
                focusedInteractable.Value = candidates[0];
                return;
            }

            var cameraTransform = new Pose(mainCamera.Position, mainCamera.Rotation);
            focusedInteractable.Value = FindBestCandidate(candidates, cameraTransform);
        }

        IInteractable FindBestCandidate(IReadOnlyList<IInteractable> candidates, Pose cameraPose)
        {
            IInteractable bestCandidate = null;
            float bestScore = float.MinValue;

            foreach (var candidate in candidates)
            {
                var directionToCandidate = (candidate.Position - cameraPose.position).normalized;
                float dot = Vector3.Dot(cameraPose.forward, directionToCandidate);
                if (dot > bestScore)
                {
                    bestScore = dot;
                    bestCandidate = candidate;
                }
            }

            return bestCandidate;
        }
    }
}