using ARMeasurementApp.Scripts.Util.Enums;
using UnityEngine;

namespace ARMeasurementApp.Scripts.Events.InputEvents
{
    public class ButtonEvent
    {
        public readonly Event ResetCurrentScene = new();

        public readonly Event ToggleContinuousMeasurementMode = new();
        public readonly Event DeleteLatestMeasurementPoint = new();

        public readonly Event EnableARPlaneDetection = new();
        public readonly Event DisableARPlaneDetection = new();

        public readonly Event ToggleConfidenceValueFiltering = new();

        public readonly Event<float> OnSliderValueChanged = new();

        public readonly Event<Vector3> RotateObject = new();

        public readonly Event ToggleAnchorPointUsed = new();
        public readonly Event ChangeAnchorPointCalculationMode = new();
    }
}