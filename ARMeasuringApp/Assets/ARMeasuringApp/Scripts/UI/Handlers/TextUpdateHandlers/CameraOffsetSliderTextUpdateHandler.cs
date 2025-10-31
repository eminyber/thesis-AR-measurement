using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using TMPro;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class CameraOffsetSliderTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<float>
    {
        [SerializeField] TMP_Text _cameraOffsetSliderText;

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
            if (_cameraOffsetSliderText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in CameraOffsetSliderTextUpdateHandler: _cameraOffsetSliderText is null");
            enabled = false;
        }

        public void UpdateText(float value)
        {
            if (value == 0)
            {
                _cameraOffsetSliderText.text = string.Empty;
                return;
            }

            _cameraOffsetSliderText.text = "Offset: " + value.ToString() + "m";
        }
    }
}

