using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class ConfidenceValueSliderTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<float>
    {
        [SerializeField] TMP_Text _confidenceValueSliderText;

        void OnEnable()
        {
            EventManager.ButtonClickEvent.OnSliderValueChanged.AddListener(UpdateText);
        }

        void OnDisable()
        {
            EventManager.ButtonClickEvent.OnSliderValueChanged.RemoveListener(UpdateText);
        }

        void Start()
        {
            if (_confidenceValueSliderText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in ConfidenceValueSliderTextUpdateHandler: _confidenceValueSliderText is null");
            enabled = false;
        }


        public void UpdateText(float value)
        {
            _confidenceValueSliderText.text = "Minimum confidence value: " + value.ToString();
        }
    }
}

