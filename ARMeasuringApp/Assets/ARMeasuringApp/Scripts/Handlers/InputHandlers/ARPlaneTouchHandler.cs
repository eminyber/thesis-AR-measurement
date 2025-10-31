using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Events.Eventargs;
using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.UI;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMeasurementApp.Scripts.Handlers.InputHandlers
{
    public class ARPlaneTouchHandler : MonoBehaviour, IUserInputHandler
    {
        [SerializeField] ARRaycastManager  _arRaycastManager;
        [SerializeField] UIElementDetector _uiElementDetector;

        private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();

        void Start() 
        {
            if (_arRaycastManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in UserPlaneInteractorController: _arRaycastManager is null");
                enabled = false;
                return;
            }

            if (_uiElementDetector == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in UserPlaneInteractorController: _uIElementDetector is null");
                enabled = false;
                return;
            }
        }
      
        public void HandleFingerDown(Vector2 screenTouchPosition)
        {
            if (!enabled) return;

            if (_uiElementDetector.IsUIElementAtPosition(screenTouchPosition)) return;

            if (_arRaycastManager.Raycast(screenTouchPosition, _raycastHits, TrackableType.PlaneWithinPolygon)) 
            {
                ARRaycastHit firstHit = _raycastHits[0];

                ARPlane hitPlane = (ARPlane)firstHit.trackable;
                EventManager.TouchEvent.UserTappedWithinARPlane.RaiseEvent(null, new PlaneAndPoseOnPlaneEventArgs(hitPlane, firstHit.pose));
            }
        }
    }
}

