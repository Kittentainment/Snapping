using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapping
{
    public class Anchor : MonoBehaviour
    {
        #region GlobalSettings

        /// <summary>
        /// It only snaps, if the angle of the other normalDirection to this Anchor's normalDirection is [180-SnappingAngle] (as normals snap to opposite direction)
        /// </summary>
        public static float SnappingAngle { get; set; } = 120;

        #endregion


        [field: SerializeField]
        [field: Range(0, 10f)]
        public float SnappingRadius { get; private set; } = 0.5f;

        [SerializeField] private Vector3 normalDirection;
        public Vector3 NormalVector => transform.rotation * normalDirection.normalized;

        /// <summary>
        /// The position of this Anchor in World Coordinates.
        ///
        /// Just returns transform.position, but now it could be changed, so an anchor can be created differently, eg just from code.
        /// </summary>
        public Vector3 AnchorPosition => transform.position;


        /// <summary>
        /// Returns a list of other Anchors which are in Range of this Anchor's SnappingRadius and do not belong to the same SnappingObj.
        /// </summary>
        /// <param name="ownAnchors">All the Anchors belonging to the same SnappingObj</param>
        /// <returns>A list of other Anchors which are in Range of this Anchor's SnappingRadius and do not belong to the same SnappingObj.</returns>
        public IEnumerable<Anchor> GetOtherAnchorsInRange(List<Anchor> ownAnchors)
        {
            // Debug.Log("GetOtherAnchorsInRange");
            ownAnchors ??= new List<Anchor>();
            var anchorsInRange = Physics.OverlapSphere(this.AnchorPosition, this.SnappingRadius)
                // Get the Anchor Components (if they exist):
                .Select(coll => coll.gameObject.GetComponent<Anchor>())
                .Where(anchor => anchor != null)
                // Only check anchors not belonging to this SnappingObj:
                .Where(anchor => !ownAnchors.Contains(anchor))
                // TODO maybe only take anchors where the angle between the two normals are big enough (no small, as they need to point in opposite directions)
                //      then we can "snap off" two objects
                .Where(anchor => Mathf.Abs(Vector3.Angle(anchor.NormalVector, NormalVector)) > 180 - SnappingAngle)
                .ToList();
            if (anchorsInRange.Count > 0)
            {
                Debug.Log("Anchor::GetOtherAnchorsInRange - Found Anchors in Range!");
            }

            return anchorsInRange;
        }


        #region Debug

        [Header("Debug Settings")] [SerializeField]
        private bool showDebugSettings = true;

        private Color _gizmosColor = Color.red;

        private Color _gizmosNormalColor = Color.green;

        [SerializeField] private float gizmosSnappingVisibility = 0.2f;

        [SerializeField] private float objectRadius = 0.2f;


        private void OnDrawGizmos()
        {
            if (!showDebugSettings)
                return;

            var position = transform.position;
            Gizmos.color = new Color(_gizmosColor.r, _gizmosColor.g, _gizmosColor.b, gizmosSnappingVisibility);
            Gizmos.DrawSphere(position, SnappingRadius);
            Gizmos.color = new Color(_gizmosColor.r, _gizmosColor.g, _gizmosColor.b, 1f);
            Gizmos.DrawSphere(position, objectRadius);
            Gizmos.color = new Color(_gizmosNormalColor.r, _gizmosNormalColor.g, _gizmosNormalColor.b, 1f);
            Gizmos.DrawLine(position, position + NormalVector);
        }

        #endregion
    }
}