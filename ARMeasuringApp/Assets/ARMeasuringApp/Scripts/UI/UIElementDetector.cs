using ARMeasurementApp.Scripts.Events;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARMeasurementApp.Scripts.UI
{
    public class UIElementDetector : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private EventSystem _eventSystem;

        void OnEnable()
        {
            if (_graphicRaycaster == null)
                _graphicRaycaster = GetComponent<GraphicRaycaster>();
            
            if (_eventSystem == null)
                _eventSystem = GetComponent<EventSystem>();
        }

        void Start()
        {
            if (_graphicRaycaster == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in UIElementDetector: _graphicRaycaster is null");
                enabled = false;
                return;
            }

            if (_eventSystem == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in UIElementDetector: _graphicRaycaster is null");
                enabled = false;
                return;
            }
        }

        public bool IsUIElementAtPosition(Vector2 position)
        {
            if (!enabled) return false;

            if (_eventSystem.IsPointerOverGameObject()) return false;

            var newPointerEventData = new PointerEventData(_eventSystem);
            newPointerEventData.position = position;

            var raycastResults = new List<RaycastResult>();
            _graphicRaycaster.Raycast(newPointerEventData, raycastResults);

            return raycastResults.Count > 0;
        }
    }
}

