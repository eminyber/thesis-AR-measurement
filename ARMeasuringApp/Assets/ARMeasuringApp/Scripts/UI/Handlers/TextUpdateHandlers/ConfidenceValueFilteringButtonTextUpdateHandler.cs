using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class ConfidenceValueFilteringButtonTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<bool>
    {
        [SerializeField] TMP_Text _confidenceValueButtonText;

        private const string CONFIDENCE_VALUE_FILTERING_ACTIVE_STR = "Deactivate Confidence Value Filtering";
        private const string CONFIDENCE_VALUE_FILTERING_INACTIVE_STR = "Activate Confidence Value Filtering";

        void OnEnable()
        {
            EventManager.UIEvent.ConfidenceValueFilteringStateChanged.AddListener(UpdateText);
        }

        void OnDisable()
        {
            EventManager.UIEvent.ConfidenceValueFilteringStateChanged.AddListener(UpdateText);
        }

        void Start()
        {
            if (_confidenceValueButtonText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in ConfidenceValueFilteringButtonTextUpdateHandler: _confidenceValueButtonText is null");
            enabled = false;
        }


        public void UpdateText(bool value)
        {
            _confidenceValueButtonText.text = value == true ? CONFIDENCE_VALUE_FILTERING_ACTIVE_STR : CONFIDENCE_VALUE_FILTERING_INACTIVE_STR;
        }
    }
}

