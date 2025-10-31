
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Util.Enums;

namespace ARMeasurementApp.Events.UIEvents
{
    public class UIEvent
    {
        public readonly Event<string> UpdateInfoText = new();

        public readonly Event<bool> ContinuousMeasurementModeStateChanged = new();
        public readonly Event<bool> ConfidenceValueFilteringStateChanged = new();
        public readonly Event<bool> UseARPlaneCenterAsAnchorPointStateChanged = new();
        public readonly Event<AnchorPointDistanceCalculationMode> AnchorPointDistanceCalculationModeChanged = new();

        public readonly Event<bool> UpdateScanButtonVisibility = new();
        public readonly Event<bool> UpdateExtensionModeUIVisibility = new();
    }
}

