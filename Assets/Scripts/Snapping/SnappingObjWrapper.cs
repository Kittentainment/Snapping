using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Snapping
{
    public class SnappingObjWrapper : MonoBehaviour
    {
        private readonly List<Anchor> _anchors = new List<Anchor>();
        
        private ObjToSnap _objToSnap;

        /// <summary>
        /// Whether this object is currently selected and being moved.
        /// </summary>
        private bool IsBeingMoved { get; set; } = false;

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
            if (IsBeingMoved)
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
            IsBeingMoved = true;
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
            if (!IsBeingMoved)
                throw new Exception("Can't let go what was not being moved in the first place!");

            if (IsSnapping)
            {
                // transform.Translate(CurrentSnapping.GetMovementVector());
                ResetObjToSnap();
                ApplySnappingToTransform(transform, CurrentSnapping);
            }

            IsBeingMoved = false;
        }

        private void ResetObjToSnap()
        {
            var objToSnapTransform = _objToSnap.transform;
            objToSnapTransform.localPosition = Vector3.zero;
            // _objToSnap.transform.rotation.Set(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w);
            objToSnapTransform.localEulerAngles = Vector3.zero;
        }

        private static void ApplySnappingToTransform(Transform transform, SnappingResult snappingResult)
        {
            var movementVector = snappingResult.GetMovementVector();
            var rotation = snappingResult.GetRotation();
            transform.Translate(movementVector, Space.World);
            RotateAround(transform, snappingResult.OtherAnchor.transform.position, rotation);
            // transform.localRotation = rotation;
        }
        
        /// <summary>
        /// From https://answers.unity.com/questions/1751620/rotating-around-a-pivot-point-using-a-quaternion.html
        /// </summary>
        /// <param name="transform">The transform to rotate</param>
        /// <param name="pivotPoint">The point to rotate it around</param>
        /// <param name="rot">The rotation to apply</param>
        static void RotateAround (Transform transform, Vector3 pivotPoint, Quaternion rot)
        {
            transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
            transform.rotation = rot * transform.rotation;
        }
        
        private void SnapToCurrentSnappingPosition()
        {
            if (CurrentSnapping == null)
                throw new NullReferenceException("We can't snap when we have nothing to snap to");
            ResetObjToSnap();
            // _objToSnap.transform.Translate(CurrentSnapping.GetMovementVector());
            ApplySnappingToTransform(_objToSnap.transform, CurrentSnapping);
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



        #region DEBUG

        private void OnDrawGizmos()
        {
            if (CurrentSnapping != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(CurrentSnapping.OwnAnchor.AnchorPosition, CurrentSnapping.OwnAnchor.AnchorPosition + CurrentSnapping.GetMovementVector());
            }
        }

        #endregion


    }
}