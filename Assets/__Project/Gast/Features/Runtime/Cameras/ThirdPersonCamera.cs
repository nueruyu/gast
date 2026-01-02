using UnityEngine;
using Cinemachine;

namespace Gast.Features.Cameras
{
    /// <summary>
    /// Third-person camera controller using Cinemachine for fixed overhead view.
    /// Uses spherical coordinates to position the camera around a target.
    /// </summary>
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

            // Set up follow and look at targets
            if (followTarget != null)
            {
                virtualCamera.Follow = followTarget;
                virtualCamera.LookAt = followTarget;
            }

            // Add and configure CinemachineTransposer for position offset
            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            UpdateCameraTransform();

            // Add and configure CinemachineComposer for look at offset
            composer = virtualCamera.AddCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = lookAtOffset;

            // Configure damping
            transposer.m_XDamping = dampingSpeed;
            transposer.m_YDamping = dampingSpeed;
            transposer.m_ZDamping = dampingSpeed;
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
            if (Application.isPlaying && transposer != null)
            {
                UpdateCameraTransform();
                UpdateLookAtOffset();
                UpdateDamping();
            }
        }

        /// <summary>
        /// Rotates the camera based on input delta.
        /// </summary>
        /// <param name="delta">The input delta from mouse or joystick.</param>
        public void Rotate(Vector2 delta)
        {
            horizontalAngle += delta.x * rotationSensitivity.x;
            verticalAngle -= delta.y * rotationSensitivity.y;

            // Clamp vertical angle
            verticalAngle = Mathf.Clamp(verticalAngle, verticalAngleLimits.x, verticalAngleLimits.y);

            // Keep horizontal angle in 0-360 range
            if (horizontalAngle > 360f) horizontalAngle -= 360f;
            if (horizontalAngle < 0f) horizontalAngle += 360f;
        }

        /// <summary>
        /// Sets the target for the camera to follow.
        /// </summary>
        /// <param name="target">The transform to follow</param>
        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            if (virtualCamera != null)
            {
                virtualCamera.Follow = target;
                virtualCamera.LookAt = target;
            }
        }

        /// <summary>
        /// Sets the look at offset from the target position.
        /// </summary>
        /// <param name="offset">Offset vector (e.g., Vector3.up * 1.5f for head height)</param>
        public void SetLookAtOffset(Vector3 offset)
        {
            lookAtOffset = offset;
            UpdateLookAtOffset();
        }

        /// <summary>
        /// Sets the camera distance from the target.
        /// </summary>
        public void SetDistance(float newDistance)
        {
            distance = Mathf.Max(0.1f, newDistance);
            UpdateCameraTransform();
        }

        /// <summary>
        /// Sets the horizontal angle around the target.
        /// </summary>
        /// <param name="angle">Angle in degrees (0 = behind, 90 = right, 180 = front)</param>
        public void SetHorizontalAngle(float angle)
        {
            horizontalAngle = angle;
            UpdateCameraTransform();
        }

        /// <summary>
        /// Sets the vertical angle from horizontal plane.
        /// </summary>
        /// <param name="angle">Angle in degrees (0 = eye level, 90 = top-down)</param>
        public void SetVerticalAngle(float angle)
        {
            verticalAngle = Mathf.Clamp(angle, verticalAngleLimits.x, verticalAngleLimits.y);
            UpdateCameraTransform();
        }

        /// <summary>
        /// Updates the camera position using spherical coordinates in world space.
        /// The camera maintains a fixed world orientation and only follows the target's position.
        /// </summary>
        void UpdateCameraTransform()
        {
            if (transposer == null)
                return;

            // Start with a base direction (behind the target in world space)
            var direction = Vector3.back;

            // Apply horizontal rotation around world Y axis
            var horizontalRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
            direction = horizontalRotation * direction;

            // Apply vertical rotation (pitch up/down)
            var right = -Vector3.Cross(Vector3.up, direction);
            var verticalRotation = Quaternion.AngleAxis(verticalAngle, right);
            direction = verticalRotation * direction;

            // Calculate final offset position in world space
            var offset = direction * distance;

            // Set binding mode to World Space so the offset is not affected by target rotation
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            transposer.m_FollowOffset = offset;
        }

        /// <summary>
        /// Updates the look at offset.
        /// </summary>
        void UpdateLookAtOffset()
        {
            if (composer != null)
            {
                composer.m_TrackedObjectOffset = lookAtOffset;
            }
        }

        /// <summary>
        /// Updates the damping settings.
        /// </summary>
        void UpdateDamping()
        {
            if (transposer != null)
            {
                transposer.m_XDamping = dampingSpeed;
                transposer.m_YDamping = dampingSpeed;
                transposer.m_ZDamping = dampingSpeed;
            }
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