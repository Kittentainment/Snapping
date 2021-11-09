using UnityEngine;

namespace Snapping
{
    public class SnappingResult
    {
        public SnappingResult(Anchor ownAnchor, Anchor otherAnchor, float distance)
        {
            OwnAnchor = ownAnchor;
            OtherAnchor = otherAnchor;
            Distance = distance;
            Debug.Log($"Created a new SnappingResult: {this}");
        }

        public readonly Anchor OwnAnchor;
        public readonly Anchor OtherAnchor;
        public readonly float Distance;

        /// <summary>
        /// The vector the ownAnchor needs to move, to be at the same position as the otherAnchor.
        /// This can then be applied to the parent GO.
        /// </summary>
        /// <returns>The vector the ownAnchor needs to move, to be at the same position as the otherAnchor.</returns>
        public Vector3 GetMovementVector()
        {
            return OtherAnchor.AnchorPosition - OwnAnchor.AnchorPosition;
        }

        public override string ToString()
        {
            return $"Distance: {Distance}";
        }
    }
}