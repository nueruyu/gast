using UnityEngine;

namespace Gast.Domain.AI
{
    public interface INavigationProvider
    {
        void SetDestination(Vector3 target);
        void Stop();
        Vector3 NextSteeringDirection { get; }
        float RemainingDistance { get; }
        bool HasArrived { get; }
        void Warp(Vector3 position);
    }
}
