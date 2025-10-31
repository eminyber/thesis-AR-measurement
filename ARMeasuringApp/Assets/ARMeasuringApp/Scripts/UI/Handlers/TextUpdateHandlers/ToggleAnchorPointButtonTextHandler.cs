using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using TMPro;
using UnityEngine;


namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class ToggleAnchorPointButtonTextHandler : MonoBehaviour, ITextUpdateHandler<bool>
    {
        [SerializeField] TMP_Text _measurementPointButtonText;

        private const string ARPLANE_CENTER_AS_ANCHOR_POINT_STR = "Plane Center";
        private const string HIT_POSITION_AS_ANCHOR_POINT_STR = "Hit Position";

        void OnEnable()
        {
            EventManager.UIEvent.UseARPlaneCenterAsAnchorPointStateChanged.AddListener(UpdateText);
        }

        void OnDisable()
        {
            EventManager.UIEvent.UseARPlaneCenterAsAnchorPointStateChanged.RemoveListener(UpdateText);
        }

        void Start()
        {
            if (_measurementPointButtonText != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in AnchorPointMeasurementPointButtonTextUpdateHandler: _measurementPointButtonText is null");
            enabled = false;

        }

        public void UpdateText(bool value)
        {
            _measurementPointButtonText.text = value == true ? ARPLANE_CENTER_AS_ANCHOR_POINT_STR : HIT_POSITION_AS_ANCHOR_POINT_STR;
        }
    }
}

