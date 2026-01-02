using UnityEngine;
using Cinemachine;

namespace Gast.Features.Cameras
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField]
        Transform followTarget;

        [SerializeField, Tooltip("Offset from target position to look at (e.g., head height)")]
        Vector3 lookAtOffset = new(0f, 1.5f, 0f);

        [Header("Camera Position (Spherical Coordinates)")]
        [SerializeField, Tooltip("Distance from the target")]
        float distance = 10f;

        [SerializeField, Tooltip("Horizontal angle around the target (0 = behind, 90 = right side, 180 = front)")]
        [Range(0f, 360f)]
        float horizontalAngle = 0f;

        [SerializeField, Tooltip("Vertical angle from horizontal plane (0 = eye level, 90 = top-down)")]
        [Range(-89f, 89f)]
        float verticalAngle = 45f;

        [Header("Rotation Settings")]
        [SerializeField]
        Vector2 rotationSensitivity = new(2.0f, 2.0f);

        [SerializeField]
        Vector2 verticalAngleLimits = new(-30f, 80f);

        [Header("Damping Settings")]
        [SerializeField, Tooltip("How smoothly the camera follows the target")]
        float dampingSpeed = 3f;

        CinemachineVirtualCamera virtualCamera;
        CinemachineTransposer transposer;
        CinemachineComposer composer;

        void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

            if (followTarget != null)
            {
                virtualCamera.Follow = followTarget;
                virtualCamera.LookAt = followTarget;
            }

            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            UpdateCameraTransform();

            composer = virtualCamera.AddCinemachineComponent<CinemachineComposer>();
            UpdateLookAtOffset();

            UpdateDamping();
        }

        void LateUpdate()
        {
            if (Application.isPlaying)
            {
                UpdateCameraTransform();
            }
        }

        void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateCameraTransform();
                UpdateLookAtOffset();
                UpdateDamping();
            }
        }

        public void Rotate(Vector2 delta)
        {
            horizontalAngle += delta.x * rotationSensitivity.x;
            verticalAngle -= delta.y * rotationSensitivity.y;

            verticalAngle = Mathf.Clamp(verticalAngle, verticalAngleLimits.x, verticalAngleLimits.y);

            if (horizontalAngle > 360f)
                horizontalAngle -= 360f;
            if (horizontalAngle < 0f)
                horizontalAngle += 360f;
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
        }

        public void SetLookAtOffset(Vector3 offset)
        {
            lookAtOffset = offset;
            UpdateLookAtOffset();
        }

        public void SetDistance(float newDistance)
        {
            distance = Mathf.Max(0.1f, newDistance);
            UpdateCameraTransform();
        }

        public void SetHorizontalAngle(float angle)
        {
            horizontalAngle = angle;
            UpdateCameraTransform();
        }

        public void SetVerticalAngle(float angle)
        {
            verticalAngle = Mathf.Clamp(angle, verticalAngleLimits.x, verticalAngleLimits.y);
            UpdateCameraTransform();
        }

        void UpdateCameraTransform()
        {
            var direction = Vector3.back;

            var horizontalRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
            direction = horizontalRotation * direction;

            var right = -Vector3.Cross(Vector3.up, direction);
            var verticalRotation = Quaternion.AngleAxis(verticalAngle, right);
            direction = verticalRotation * direction;

            var offset = direction * distance;

            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            transposer.m_FollowOffset = offset;
        }

        void UpdateLookAtOffset()
        {
            composer.m_TrackedObjectOffset = lookAtOffset;
        }

        void UpdateDamping()
        {
            transposer.m_XDamping = dampingSpeed;
            transposer.m_YDamping = dampingSpeed;
            transposer.m_ZDamping = dampingSpeed;
            composer.m_HorizontalDamping = 0;
            composer.m_VerticalDamping = 0;
        }

        void OnDrawGizmosSelected()
        {
            if (followTarget == null)
                return;

            // Calculate camera position in world space using same logic as UpdateCameraTransform
            var direction = Vector3.back;
            var horizontalRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
            direction = horizontalRotation * direction;

            var right = -Vector3.Cross(Vector3.up, direction);
            var verticalRotation = Quaternion.AngleAxis(verticalAngle, right);
            direction = verticalRotation * direction;

            var offset = direction * distance;

            var targetPos = followTarget.position;
            var lookAtPos = targetPos + lookAtOffset;
            var cameraPos = targetPos + offset;

            // Draw gizmos
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lookAtPos, 0.3f); // Look at point

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cameraPos, 0.5f); // Camera position
            Gizmos.DrawLine(lookAtPos, cameraPos); // Line from look at to camera

            Gizmos.color = Color.red;
            Gizmos.DrawLine(targetPos, lookAtPos); // Look at offset
        }
    }
}