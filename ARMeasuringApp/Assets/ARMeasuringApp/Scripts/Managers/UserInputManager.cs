using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace ARMeasurementApp.Scripts.Managers
{
    public class UserInputManager : MonoBehaviour
    {
        private IUserInputHandler _userInputHandler;
        void OnEnable()
        {
            _userInputHandler = GetComponent<IUserInputHandler>();
            
            EnhancedTouch.EnhancedTouchSupport.Enable();
            EnhancedTouch.Touch.onFingerDown += OnFingerDown;
        }

        void OnDisable()
        {
            EnhancedTouch.EnhancedTouchSupport.Disable();
            EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
        }

        void Start()
        {
            if (_userInputHandler != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in UserInputManager: Could not find an IUserInputHandler");
            enabled = false;
        }

        private void OnFingerDown(EnhancedTouch.Finger finger)
        {
            if (finger.index != 0) return;

            _userInputHandler.HandleFingerDown(finger.screenPosition);
        }
    }
}