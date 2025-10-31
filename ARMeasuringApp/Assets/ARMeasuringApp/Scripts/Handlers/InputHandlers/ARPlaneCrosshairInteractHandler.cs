using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Events.Eventargs;
using ARMeasurementApp.Scripts.Services;
using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.UI;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMeasurementApp.Scripts.Handlers.InputHandlers
{
    public class ARPlaneCrosshairInteractHandler : MonoBehaviour, IUserInputHandler
    {
        [SerializeField] GameObject _crossHair;

        [SerializeField] Camera _arCamera;
        [SerializeField] ARRaycastManager _arRaycastManager;
        [SerializeField] UIElementDetector _uIElementDetector;

        [SerializeField] bool _canSelectSelectableObjects;
        [SerializeField] bool _sendBothPlaneAndPose;

        private SelectableObjectDetector _selectableObjectDetector = new SelectableObjectDetector();

        private Pose _currentHitPose;
        private ARPlane _currentHitPlane;

        private Vector2 _middleOfScreenPosition;

        private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

        void OnEnable()
        {
            _middleOfScreenPosition  = new Vector2(Screen.width / 2, Screen.height / 2);

            if (_crossHair != null)
                _crossHair.SetActive(false);
        }

        void Start()
        {
            if (_crossHair == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CrosshairInteractorController: _crossHair is null");
                enabled = false;
                return;
            }

            if (_arCamera == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CrosshairInteractorController: _arCamera is null");
                enabled = false;
                return;
            }

            if (_arRaycastManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CrosshairInteractorController: _arRaycastManager is null");
                enabled = false;
                return;
            }

            if (_uIElementDetector == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CrosshairInteractorController: _uIElementDetector is null");
                enabled = false;
                return;
            }
        }

        // If needed due to performance, try to limit the number of updates while still keeping it responsive. 
        void Update()
        {
            if (_arRaycastManager.Raycast(_middleOfScreenPosition, _hits, TrackableType.PlaneWithinPolygon))
            {
                _currentHitPose = _hits[0].pose;
                _currentHitPlane = (ARPlane)_hits[0].trackable;

                UpdateCrosshairPosition(_currentHitPose);

                if (!_crossHair.activeInHierarchy)
                    _crossHair.SetActive(true);
            }
            else
            {
                _crossHair.SetActive(false);
            }
        }

        public void HandleFingerDown(Vector2 screenTouchPosition)
        {
            if (!enabled) return;

            if (!_crossHair.activeInHierarchy) return;

            if (_uIElementDetector.IsUIElementAtPosition(screenTouchPosition)) return;

            if (_canSelectSelectableObjects) 
            {
                if (WasSelectableObjectHit())
                {
                    EventManager.TouchEvent.OnUserInteractedWithSelectableObject.RaiseEvent();
                }
            }

            if (_sendBothPlaneAndPose)
            {
                EventManager.TouchEvent.UserTappedWithinARPlane.RaiseEvent(null, new PlaneAndPoseOnPlaneEventArgs(_currentHitPlane, _currentHitPose));
            }
            else
            {
                EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.RaiseEvent(_currentHitPose);
            }
        }

        private void UpdateCrosshairPosition(Pose hit)
        {
            _crossHair.transform.position = hit.position;
            _crossHair.transform.rotation = hit.rotation;
        }

        private bool WasSelectableObjectHit()
        {
            return _selectableObjectDetector.IsSelectableObjectAtPosition(_middleOfScreenPosition, _arCamera);
        }
    }
}

