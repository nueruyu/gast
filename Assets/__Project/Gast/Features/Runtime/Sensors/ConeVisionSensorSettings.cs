using UnityEngine;

namespace Gast.Features.Sensors
{
    [CreateAssetMenu(fileName = "ConeVisionSensorSettings", menuName = "Gast/Sensors/Cone Vision Sensor Settings")]
    public class ConeVisionSensorSettings : ScriptableObject
    {
        [SerializeField]
        float viewRadius = 30f;

        [SerializeField]
        float viewAngle = 180f;

        [SerializeField]
        Vector3 eyeOffset = new(0, 1.5f, 0);

        public float ViewRadius => viewRadius;
        public float ViewAngle => viewAngle;
        public Vector3 EyeOffset => eyeOffset;
    }
}
