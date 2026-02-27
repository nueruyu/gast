using Gast.Domain.AI;
using UnityEngine;
using UnityEngine.AI;

namespace Gast.Unity.Features.Navigations
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshNavigator : MonoBehaviour, INavigationProvider
    {
        NavMeshAgent agent;

        public Vector3 NextSteeringDirection
        {
            get
            {
                if (!agent.hasPath)
                    return Vector3.zero;

                var direction = agent.steeringTarget - transform.position;
                direction.y = 0; // Flatten Y to prevent tilting
                return direction.normalized;
            }
        }

        public float RemainingDistance => agent.remainingDistance;
        public bool HasArrived => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

        public float StoppingDistance
        {
            get => agent.stoppingDistance;
            set => agent.stoppingDistance = value;
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            // Ghost Agent pattern: decouple agent from transform
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        void Update()
        {
            // Sync agent position to current transform position
            agent.nextPosition = transform.position;
        }

        public void SetDestination(Vector3 target)
        {
            if (!agent)
                return;
            agent.SetDestination(target);
        }

        public void Stop()
        {
            if (!agent)
                return;
            agent.ResetPath();
        }

        public void Warp(Vector3 position)
        {
            if (!agent)
                return;
            agent.Warp(position);
        }

        void OnDrawGizmosSelected()
        {
            if (agent == null || !agent.hasPath)
                return;

            Gizmos.color = Color.cyan;
            var path = agent.path;
            for (var i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(agent.destination, 0.3f);
        }
    }
}