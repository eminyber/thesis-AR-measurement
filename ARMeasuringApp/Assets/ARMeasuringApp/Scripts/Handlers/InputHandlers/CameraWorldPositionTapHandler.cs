using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.UI;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Handlers.InputHandlers
{
    public class CameraWorldPositionTapHandler : MonoBehaviour, IUserInputHandler
    {
        [SerializeField] Camera _arCamera;
        [SerializeField] UIElementDetector _uiElementDetector;

        private float _cameraOffset = 0f;

        void OnEnable()
        {
            EventManager.ButtonClickEvent.OnSliderValueChanged.AddListener(UpdateCameraOffset);
        }

        void OnDisable() 
        {
            EventManager.ButtonClickEvent.OnSliderValueChanged.RemoveListener(UpdateCameraOffset);
        }

        void Start()
        {
            if (_arCamera == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CameraPositionInteractorController: _arCamera is null.");
                enabled = false;
                return;
            }

            if (_uiElementDetector == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CameraPositionInteractorController: _uiElementDetector is null.");
                enabled = false;
                return;
            }
        }

        void IUserInputHandler.HandleFingerDown(Vector2 screenPosition)
        {
            if (!enabled) return;

            if (_uiElementDetector.IsUIElementAtPosition(screenPosition)) return;

            var cameraPositionWithOffset = _arCamera.transform.position + (_arCamera.transform.forward * _cameraOffset);

            var currentWorldCameraPose = new Pose(cameraPositionWithOffset, _arCamera.transform.rotation);
            EventManager.TouchEvent.UserTappedToSendCameraPose.RaiseEvent(currentWorldCameraPose);
        }

        private void UpdateCameraOffset(float newOffset) 
        {
            _cameraOffset = newOffset;
        }
    }
}

