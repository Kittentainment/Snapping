using System;
using JetBrains.Annotations;
using Snapping;
using UnityEngine;

namespace Moving
{
    public class MoveObjectController : MonoBehaviour
    {
        [CanBeNull] private SnappingObj _selection;

        public void SelectAnObject([NotNull] SnappingObj obj)
        {
            _selection = obj ? obj : throw new ArgumentNullException(nameof(obj));
            Debug.Log($"selected an object: {_selection.gameObject.name}");
        }

        public void DeselectObject()
        {
            _selection = null;
        }
        
        
        
    }
}
