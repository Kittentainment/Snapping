using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapping
{
    public class SnappingObj : MonoBehaviour
    {
        private readonly List<Anchor> _anchors = new List<Anchor>();

        [SerializeField] private bool isBeingMoved = false;

        private void Awake()
        {
            _anchors.AddRange(gameObject.GetComponentsInChildren<Anchor>());
            Debug.Log(_anchors.Count);
        }


        private void FixedUpdate()
        {
            if (isBeingMoved)
            {
                // Debug.Log("Searching Snapping Points");
                var nearestSnapping = _anchors
                    .SelectMany(ownAnchor => ownAnchor.GetOtherAnchorsInRange(_anchors)
                        .Select(otherAnchor => new SnappingResult(ownAnchor, otherAnchor,
                            Vector3.Distance(ownAnchor.AnchorPosition, otherAnchor.AnchorPosition)))
                    )
                    .OrderBy(result => result.Distance)
                    .FirstOrDefault();

                if (nearestSnapping != null)
                {
                    Debug.Log($"Found Snapping: {nearestSnapping}");
                }
                
            }
        }

        private class SnappingResult
        {
            public SnappingResult(Anchor ownAnchor, Anchor otherAnchor, float distance)
            {
                this.OwnAnchor = ownAnchor;
                this.OtherAnchor = otherAnchor;
                this.Distance = distance;
                Debug.Log($"Created a new SnappingResult: {this}");
            }

            public readonly Anchor OwnAnchor;
            public readonly Anchor OtherAnchor;
            public readonly float Distance;

            public override string ToString()
            {
                return $"Distance: {Distance}";
            }
        }

    }
}