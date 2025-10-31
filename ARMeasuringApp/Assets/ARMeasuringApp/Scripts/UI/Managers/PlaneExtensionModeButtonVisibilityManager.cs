using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.UI;

namespace ARMeasurementApp.Scripts.UI.Managers
{
    public class PlaneExtensionModeButtonVisibilityManager : MonoBehaviour
    {
        [SerializeField] Button _planeExtensionModeButton;

        void OnEnable()
        {
            EventManager.UIEvent.UpdateExtensionModeUIVisibility.AddListener(SetButtonVisibility);
        }

        void OnDisable() 
        {
            EventManager.UIEvent.UpdateExtensionModeUIVisibility.RemoveListener(SetButtonVisibility);
        }

        void Start()
        {
            if (_planeExtensionModeButton != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneExtensionModeButtonVisibilityManager: _planeExtensionModeButton is null");
            enabled = false;

        }

        private void SetButtonVisibility(bool shouldBeActive)
        {
            _planeExtensionModeButton.gameObject.SetActive(shouldBeActive);
        }
    }
}

