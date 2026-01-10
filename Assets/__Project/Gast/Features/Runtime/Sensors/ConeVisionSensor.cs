using System.Collections.Generic;
using Gast.Domain.Characters;
using Gast.Domain.Sensors;
using Gast.Shared.UnityExtensions;
using UnityEngine;

namespace Gast.Features.Sensors
{
    public class ConeVisionSensor : MonoBehaviour, IVisionSensor
    {
        [SerializeField]
        float viewRadius = 10f;

        [SerializeField]
        float viewAngle = 90f;

        [SerializeField]
        LayerMask targetMask;

        [SerializeField]
        LayerMask obstacleMask;

        [SerializeField]
        Vector3 eyeOffset = new(0, 1.5f, 0);

        readonly List<ICharacter> visibleCharacters = new();
        readonly Collider[] overlapBuffer = new Collider[32];

        ICharacter self;

        public IReadOnlyList<ICharacter> VisibleCharacters => visibleCharacters;
        public Vector3 EyePosition => transform.position + eyeOffset;

        public float ViewRadius
        {
            get => viewRadius;
            set => viewRadius = value;
        }

        public float ViewAngle
        {
            get => viewAngle;
            set => viewAngle = value;
        }

        public Vector3 EyeOffset
        {
            get => eyeOffset;
            set => eyeOffset = value;
        }

        void Start()
        {
            self = this.RequireComponentInParent<ICharacter>();
        }

        void FixedUpdate()
        {
            visibleCharacters.Clear();

            var eyePos = EyePosition;
            var count = Physics.OverlapSphereNonAlloc(eyePos, viewRadius, overlapBuffer, targetMask);

            for (var i = 0; i < count; i++)
            {
                var target = overlapBuffer[i].transform;

                if (target == transform)
                    continue;

                if (!target.TryGetComponent<ICharacter>(out var character))
                    continue;

                if (character == self)
                    continue;

                var dirToTarget = (target.position - eyePos).normalized;
                var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
                if (angleToTarget > viewAngle / 2f)
                    continue;

                var distToTarget = Vector3.Distance(eyePos, target.position);
                if (Physics.Raycast(eyePos, dirToTarget, distToTarget, obstacleMask))
                    continue;

                visibleCharacters.Add(character);
            }
        }

        public bool IsVisible(ICharacter target)
        {
            return visibleCharacters.Contains(target);
        }

        void OnDrawGizmosSelected()
        {
            var eyePos = EyePosition;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePos, viewRadius);

            var leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
            var rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(eyePos, eyePos + leftBoundary * viewRadius);
            Gizmos.DrawLine(eyePos, eyePos + rightBoundary * viewRadius);

            Gizmos.color = Color.red;
            foreach (var character in visibleCharacters)
            {
                if (character is MonoBehaviour mb)
                {
                    Gizmos.DrawLine(eyePos, mb.transform.position);
                }
            }
        }
    }
}