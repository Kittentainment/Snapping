using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Snapping
{
    public class SnappingObj : MonoBehaviour
    {
        private readonly List<Anchor> _anchors = new List<Anchor>();
        
        private ObjToSnap _objToSnap;

        [SerializeField] private bool isBeingMoved = false;
        private bool isSnapping = false;

        private void Awake()
        {
            _objToSnap ??= GetComponentInChildren<ObjToSnap>();
            _anchors.AddRange(gameObject.GetComponentsInChildren<Anchor>());
            if (!_objToSnap.transform.localPosition.Equals(Vector3.zero))
            {
                Debug.LogWarning($"The ObjToSnap should be at (0, 0, 0) instead of {transform.localPosition} (relative to it's parent object)");
                ResetObjToSnap();
            }

            Debug.Log($"Anchors Found: {_anchors.Count}");
        }


        private void FixedUpdate()
        {
            if (isBeingMoved)
            {
                var nearestSnapping = GetNearestSnapping();

                if (nearestSnapping != null)
                {
                    Debug.Log($"Found Snapping: {nearestSnapping}");
                    SnapToPosition(nearestSnapping);
                } else if (isSnapping)
                {
                    ResetObjToSnap();
                }
            }
        }
        
        
        private void LetGoAndSnap()
        {
            if (!isBeingMoved)
                throw new Exception("Can't let go what was not being moved in the first place!");
            
            // TODO
            
            isBeingMoved = false;
        }

        private void ResetObjToSnap()
        {
            _objToSnap.transform.localPosition = Vector3.zero;
        }


        [CanBeNull]
        private SnappingResult GetNearestSnapping()
        {
            return _anchors
                .SelectMany(ownAnchor => ownAnchor.GetOtherAnchorsInRange(_anchors)
                    .Select(otherAnchor => new SnappingResult(ownAnchor, otherAnchor,
                        Vector3.Distance(ownAnchor.AnchorPosition, otherAnchor.AnchorPosition)))
                )
                .OrderBy(result => result.Distance)
                .FirstOrDefault();
        }

        private void SnapToPosition(SnappingResult nearestSnapping)
        {
            ResetObjToSnap();
            _objToSnap.transform.Translate(nearestSnapping.GetMovementVector());
            isSnapping = true;
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
}