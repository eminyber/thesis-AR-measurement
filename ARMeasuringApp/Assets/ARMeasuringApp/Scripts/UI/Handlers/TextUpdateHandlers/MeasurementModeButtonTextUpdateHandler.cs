using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class MeasurementModeButtonTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<bool>
    {
        [SerializeField] TMP_Text _measurementModeButtonText;

        private const string CONTINUOUS_MEASUREMENT_MODE_STR = "Continuous Measurements";
        private const string SEPARATE_MEASUREMENT_MODE_STR = "Separate Measurements";

        void OnEnable()
        {
            EventManager.UIEvent.ContinuousMeasurementModeStateChanged.AddListener(UpdateText);
        }

        void OnDisable()
        {
            EventManager.UIEvent.ContinuousMeasurementModeStateChanged.RemoveListener(UpdateText);
        }

        void Start()
        {
            if (_measurementModeButtonText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in _measurementModeButtonText: _measurementModeButtonText is null");
            enabled = false;
        }

        public void UpdateText(bool value)
        {
            _measurementModeButtonText.text = value == true ? CONTINUOUS_MEASUREMENT_MODE_STR : SEPARATE_MEASUREMENT_MODE_STR;
        }
    }
}

