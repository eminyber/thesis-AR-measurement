using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class PlaneLevelOffsetSliderTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<float>
    {
        [SerializeField] TMP_Text _planeLevelOffsetSliderText;

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
            if (_planeLevelOffsetSliderText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneLevelOffsetSliderTextUpdateHandler: _planeLevelOffsetSliderText is null");
            enabled = false;
        }

        public void UpdateText(float value)
        {
            _planeLevelOffsetSliderText.text = "Plane Level Offset: " + value + "m";
        }
    }
}

