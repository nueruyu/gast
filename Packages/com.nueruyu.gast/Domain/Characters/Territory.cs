using UnityEngine;

namespace Gast.Domain.Characters
{
    /// <summary>
    ///     Represents a character's territory, defined by a home position and a radius.
    /// </summary>
    public readonly struct Territory
    {
        public Vector3 HomePosition { get; }
        public float Radius { get; }

        public Territory(Vector3 homePosition, float radius)
        {
            HomePosition = homePosition;
            Radius = radius;
        }

        /// <summary>
        ///     Checks if a given position is outside the territory.
        /// </summary>
        /// <param name="currentPosition">The position to check.</param>
        /// <returns>True if the position is outside the territory, false otherwise.</returns>
        public bool IsOutOfTerritory(Vector3 currentPosition)
        {
            var horizontalDistanceSq = (new Vector2(currentPosition.x, currentPosition.z) -
                                        new Vector2(HomePosition.x, HomePosition.z)).sqrMagnitude;
            return horizontalDistanceSq > Radius * Radius;
        }

        /// <summary>
        ///     Checks if a given position is safely inside the territory with a margin,
        ///     used as the return condition when coming back to avoid boundary oscillation.
        /// </summary>
        public bool IsInsideTerritoryWithMargin(Vector3 currentPosition, float margin)
        {
            var innerRadius = Radius - margin;
            var horizontalDistanceSq = (new Vector2(currentPosition.x, currentPosition.z) -
                                        new Vector2(HomePosition.x, HomePosition.z)).sqrMagnitude;
            return horizontalDistanceSq <= innerRadius * innerRadius;
        }
    }
}