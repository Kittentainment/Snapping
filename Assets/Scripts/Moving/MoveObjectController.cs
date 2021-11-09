using System;
using JetBrains.Annotations;
using Snapping;
using UnityEngine;

namespace Moving
{
    public class MoveObjectController : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1;
        
        [CanBeNull] private SnappingObj _selection;

        public void SelectAnObject([NotNull] SnappingObj obj)
        {
            DeselectObject();
            _selection = obj ? obj : throw new ArgumentNullException(nameof(obj));
            _selection.MovementHasStarted();
            Debug.Log($"selected an object: {_selection.gameObject.name}");
        }

        public void DeselectObject()
        {
            if (_selection != null) _selection.LetGoAndSnap();
            _selection = null;
        }

        public void MoveSelection(Vector3 movement, float deltaTime)
        {
            Debug.Log($"MoveObjectController::MoveSelection({movement}, {deltaTime})");
            if (_selection != null)
            {
                _selection.transform.Translate(movement * deltaTime * movementSpeed);
            }
        }
        
    }
}
