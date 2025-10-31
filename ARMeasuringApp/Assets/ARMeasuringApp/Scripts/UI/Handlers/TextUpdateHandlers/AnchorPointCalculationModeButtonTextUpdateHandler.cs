using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Util.Enums;
using ARMeasurementApp.Scripts.Util.Formatters;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class AnchorPointCalculationModeButtonTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<AnchorPointDistanceCalculationMode>
    {
        [SerializeField] TMP_Text _calculationModeButtonText;

        private IEnumToStringFormatter<AnchorPointDistanceCalculationMode> _anchorPointDistanceCalculationModeFormatter = new AnchorPointDistanceCalculationModeEnumFormatter();

        void OnEnable()
        {
            EventManager.UIEvent.AnchorPointDistanceCalculationModeChanged.AddListener(UpdateText);
        }

        void OnDisable()
        {
            EventManager.UIEvent.AnchorPointDistanceCalculationModeChanged.RemoveListener(UpdateText);
        }

        void Start()
        {
            if (_calculationModeButtonText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in AnchorPointCalculationModeButtonTextUpdateHandler: _calculationModeButtonText is null");
            enabled = false;
        }

        public void UpdateText(AnchorPointDistanceCalculationMode value)
        {
            var resolvedString = _anchorPointDistanceCalculationModeFormatter.ToString(value);
            _calculationModeButtonText.text = resolvedString;
        }
    }
}
