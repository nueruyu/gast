using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Application.Items;
using Gast.Core.Commands;
using Gast.Core.Tasks;
using Gast.Domain.Cameras;
using Gast.Domain.Inputs;
using Gast.Domain.Items;
using Gast.Domain.Players;
using UnityEngine;

namespace Gast.Unity.Features.Placement
{
    public class PlacementController : ILifecycleTask
    {
        readonly PlacementService placementService;
        readonly PlacementSettings settings;
        readonly IInputProvider input;
        readonly ICameraService cameraService;
        readonly IPlacedItemFactory placedItemFactory;
        readonly IPlayerManager playerManager;
        readonly ICommandDispatcher commandDispatcher;

        GameObject previewInstance;
        float currentYRotation;
        bool isValidPlacement;

        public PlacementController(
            PlacementService placementService,
            PlacementSettings settings,
            IInputProvider input,
            ICameraService cameraService,
            IPlacedItemFactory placedItemFactory,
            IPlayerManager playerManager,
            ICommandDispatcher commandDispatcher)
        {
            this.placementService = placementService;
            this.settings = settings;
            this.input = input;
            this.cameraService = cameraService;
            this.placedItemFactory = placedItemFactory;
            this.playerManager = playerManager;
            this.commandDispatcher = commandDispatcher;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            placementService.IsActive.Subscribe(active =>
            {
                if (active)
                    OnEnterMode();
                else
                    OnExitMode();
            }).AddTo(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (placementService.IsActive.Value)
                {
                    UpdatePlacement();
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        void OnEnterMode()
        {
            var itemId = placementService.CurrentItemId.Value;
            previewInstance = placedItemFactory.CreatePreview(itemId);

            if (previewInstance == null)
            {
                placementService.ExitMode();
                return;
            }

            currentYRotation = 0f;
            SetPreviewMaterial(settings.InvalidPlacementMaterial);
        }

        void OnExitMode()
        {
            if (previewInstance != null)
            {
                Object.Destroy(previewInstance);
                previewInstance = null;
            }
        }

        void UpdatePlacement()
        {
            if (previewInstance == null) return;

            UpdatePreviewTransform();

            if (input.Rotate)
            {
                currentYRotation = (currentYRotation + 90f) % 360f;
            }

            if (input.Cancel)
            {
                placementService.ExitMode();
                return;
            }

            if (input.Place && isValidPlacement)
            {
                PlaceItem();
                placementService.ExitMode();
            }
        }

        void UpdatePreviewTransform()
        {
            var cam = cameraService.MainCamera;
            var ray = new Ray(cam.Position, cam.Rotation * Vector3.forward);

            if (Physics.Raycast(ray, out var hit, settings.MaxPlacementDistance, settings.PlacementLayerMask))
            {
                previewInstance.SetActive(true);
                var rotation = Quaternion.Euler(0, currentYRotation, 0);
                previewInstance.transform.SetPositionAndRotation(hit.point, rotation);

                // Use renderer bounds for overlap check (colliders are disabled on preview)
                var bounds = CalculateRendererBounds(previewInstance);
                isValidPlacement = !Physics.CheckBox(
                    bounds.center,
                    bounds.extents,
                    rotation,
                    settings.PlacementLayerMask,
                    QueryTriggerInteraction.Ignore);

                SetPreviewMaterial(isValidPlacement
                    ? settings.ValidPlacementMaterial
                    : settings.InvalidPlacementMaterial);
            }
            else
            {
                previewInstance.SetActive(false);
                isValidPlacement = false;
            }
        }

        void PlaceItem()
        {
            var character = playerManager.CurrentCharacter.Value;
            if (character == null) return;

            commandDispatcher.Dispatch<PlaceItemCommand, bool>(new PlaceItemCommand(
                character.Id,
                placementService.CurrentItemId.Value,
                previewInstance.transform.position,
                previewInstance.transform.rotation));
        }

        void SetPreviewMaterial(Material material)
        {
            if (previewInstance == null || material == null) return;
            foreach (var renderer in previewInstance.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.sharedMaterials;
                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }
                renderer.materials = materials;
            }
        }

        static Bounds CalculateRendererBounds(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(obj.transform.position, Vector3.one * 0.5f);

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
    }
}
