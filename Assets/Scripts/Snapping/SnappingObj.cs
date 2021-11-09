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

        /// <summary>
        /// Whether this object is currently selected and being moved.
        /// </summary>
        [SerializeField] private bool isBeingMoved = false;
        
        public bool IsSnapping => CurrentSnapping != null;
        [CanBeNull] public SnappingResult CurrentSnapping { get; private set; }

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
                UpdateCurrentSnapping();
            }
        }


        /// <summary>
        /// Tells the object that it has been selected and is now being moved.
        /// From now on it will check for snapping and snap to places until let go again with LetGoAndSnap.
        /// </summary>
        public void MovementHasStarted()
        {
            isBeingMoved = true;
        }
        
        /// <summary>
        /// Let's the object go (deselect it) and tells it to stay where it is (or be moved by physics, depending on the
        /// object, and the rigidbody etc., but not by the player anymore).
        /// If it is currently in snapping range of an other object, it will now snap permanently to it (TODO with a fixed joint),
        /// and reset the wrapped object to it's parents origin (but keeping it in place, as we move the wrapper as well).
        ///
        /// If we ever want to move two pieces simultaneously (maybe left and right hand in VR), we can check here if it
        /// would snap to the piece in the other hand if let go.
        /// </summary>
        /// <exception cref="Exception">Thrown if the object has not been held in the first place.</exception>
        public void LetGoAndSnap()
        {
            if (!isBeingMoved)
                throw new Exception("Can't let go what was not being moved in the first place!");
            //TODO
            
            
            isBeingMoved = false;
        }

        private void ResetObjToSnap()
        {
            _objToSnap.transform.localPosition = Vector3.zero;
        }


        [CanBeNull]
        private void UpdateCurrentSnapping()
        {
            var wasSnappingBefore = IsSnapping;

            CurrentSnapping = GetNearestSnapping();
            
            if (CurrentSnapping != null) Debug.Log($"Found Snapping: {CurrentSnapping}");
            
            
            if (CurrentSnapping != null)
            {
                SnapToCurrentSnappingPosition();
            } else if (wasSnappingBefore)
            {
                // If we were snapping before, but now aren't (meaning we left the radius of the anchor) then we want to
                // reset it again to the origin of the wrapper.
                ResetObjToSnap();
            }
        }

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

        private void SnapToCurrentSnappingPosition()
        {
            ResetObjToSnap();
            _objToSnap.transform.Translate(CurrentSnapping.GetMovementVector());
        }


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
}