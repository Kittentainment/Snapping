using UnityEngine;

namespace Snapping
{
    public class ObjToSnap : MonoBehaviour
    {
        
        
        
        /// <summary>
        /// Returns an object to represent the position, where this object will snap to, if let go.
        /// This method can be changed and overwritten to calculate the object or clone it from a prefab.
        /// The object should preferably be somehow see-through.
        /// It will be destroyed after it's not needed anymore (snapping radius left, or let got), therefore if another
        /// object is used as a base, it has to be cloned (for example with Instantiate).
        /// </summary>
        public GameObject GetSnappingPreview
        {
            get
            {
                var previewGO = Instantiate(gameObject);
                previewGO.GetComponent<ObjToSnap>().enabled = false;
                
                return previewGO;
            }
        }
    }
}
