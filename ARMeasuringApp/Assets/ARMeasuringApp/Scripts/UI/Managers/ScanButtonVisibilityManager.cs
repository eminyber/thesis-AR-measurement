using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.UI;

namespace ARMeasurementApp.Scripts.UI.Managers
{
    public class ScanButtonVisibilityManager : MonoBehaviour
    {
        [SerializeField] Button _scanButton;

        void OnEnable()
        {
            EventManager.UIEvent.UpdateScanButtonVisibility.AddListener(SetScanButtonVisiblity);
        }

        void OnDisable()
        {
            EventManager.UIEvent.UpdateScanButtonVisibility.RemoveListener(SetScanButtonVisiblity);
        }

        void Start()
        {
            if (_scanButton != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in ScanButtonVisibilityManager: _scanButton is null");
            enabled = false;
        }
        private void SetScanButtonVisiblity(bool shouldBeActive)
        {
            _scanButton.gameObject.SetActive(shouldBeActive);
        }
    }
}

