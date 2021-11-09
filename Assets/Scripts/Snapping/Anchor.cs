using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapping
{
    public class Anchor : MonoBehaviour
    {
        [field: SerializeField]
        [field: Range(0, 10f)] public float SnappingRadius { get; private set; } = 0.5f;

        [SerializeField] private Vector3 normalDirection;
        public Vector3 NormalVector => transform.rotation * normalDirection.normalized;

        public Vector3 AnchorPosition => transform.position;
        


        #region Debug

        [Header("Debug Settings")]
    
        [SerializeField]
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


        /// <summary>
        /// Returns a list of other Anchors which are in Range of this Anchor's SnappingRadius and do not belong to the same SnappingObj.
        /// </summary>
        /// <param name="ownAnchors">All the Anchors belonging to the same SnappingObj</param>
        /// <returns>A list of other Anchors which are in Range of this Anchor's SnappingRadius and do not belong to the same SnappingObj.</returns>
        public IEnumerable<Anchor> GetOtherAnchorsInRange(List<Anchor> ownAnchors)
        {
            Debug.Log("GetOtherAnchorsInRange");
            ownAnchors ??= new List<Anchor>();
            var anchorsInRange = Physics.OverlapSphere(this.AnchorPosition, this.SnappingRadius,
                    LayerMask.NameToLayer("AnchorLayer"))
                // Get the Anchor Components (if they exist):
                .Select(coll =>
                {
                    Debug.Log(coll);
                    return coll.gameObject.GetComponent<Anchor>();
                })
                .Where(anchor => anchor != null)
                // Only check anchors not belonging to this SnappingObj:
                .Where(anchor => !ownAnchors.Contains(anchor))
                .ToList();
            if (anchorsInRange.Count > 0)
            {
                Debug.Log("Found Anchors in Range!");
            }

            return anchorsInRange;
        }
    }
}
