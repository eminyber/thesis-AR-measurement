
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.HoldButtonHandlers
{
    public class HoldToRotateObjectHandler : MonoBehaviour, IHoldButtonHandler
    {
        [SerializeField] Vector3 _rotationDirection = Vector3.up;
        [SerializeField] float _rotationAmount = 1f;

        private bool _isBeingHeldDown = false;

        public void OnButtonDown()
        {
            EventManager.ButtonClickEvent.RotateObject.RaiseEvent(_rotationDirection.normalized * _rotationAmount);

            _isBeingHeldDown = true;
        }

        public void OnButtonUp()
        {
            _isBeingHeldDown = false;
        }

        void Update()
        {
            if (_isBeingHeldDown)
                EventManager.ButtonClickEvent.RotateObject.RaiseEvent(_rotationDirection.normalized * _rotationAmount);
        }
    }
}

