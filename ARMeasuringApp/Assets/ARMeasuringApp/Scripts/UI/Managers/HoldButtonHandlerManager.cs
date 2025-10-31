using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMeasurementApp.Scripts.UI.Managers
{
    public class HoldButtonHandlerManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private IHoldButtonHandler _holdButtonHandler;

        void OnEnable()
        {
            _holdButtonHandler = GetComponent<IHoldButtonHandler>();
        }
        
        void Start()
        {
            if (_holdButtonHandler != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in HoldButtonActionManager: Could not find a IHoldButtonHandler interface");
            enabled = false;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enabled) return;

            _holdButtonHandler.OnButtonDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!enabled) return;

            _holdButtonHandler.OnButtonUp();
        }
    }
}

