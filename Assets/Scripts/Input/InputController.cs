using System;
using Moving;
using Snapping;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputController : MonoBehaviour
    {
        private MoveObjectController _moveObjectController;
        private void Awake()
        {
            _moveObjectController = FindObjectOfType<MoveObjectController>();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (Camera.main is null) return;

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            SnappingObj otherObj = null;
            if (Physics.Raycast(ray, out var hit))
            {
                var go = hit.transform.gameObject;
                var snappingObj = go.GetComponentInParent<SnappingObj>();
                if (snappingObj != null)
                {
                    otherObj = snappingObj;
                }
            }

            if (otherObj is null)
            {
                _moveObjectController.DeselectObject();
            }
            else
            {
                _moveObjectController.SelectAnObject(otherObj);
            }
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            
        }
        
    }
}
