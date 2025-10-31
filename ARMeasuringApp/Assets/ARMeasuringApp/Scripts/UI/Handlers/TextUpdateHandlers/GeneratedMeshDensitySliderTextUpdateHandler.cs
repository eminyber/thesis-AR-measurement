using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{   
    public class GeneratedMeshDensitySliderTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<float>
    {
        [SerializeField] TMP_Text _meshDensitySliderText;

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
            if (_meshDensitySliderText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in GeneratedMeshDensityTextUpdateHandler: _meshDensitySliderText is null");
            enabled = false;
        }

        public void UpdateText(float value)
        {
            _meshDensitySliderText.text = "Generated Mesh Density: " + value.ToString();
        }
    }
}

