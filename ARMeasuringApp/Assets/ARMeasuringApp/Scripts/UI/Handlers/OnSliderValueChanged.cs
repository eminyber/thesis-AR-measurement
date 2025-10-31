using ARMeasurementApp.Scripts.Events;

using System;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers
{
    public class OnSliderValueChanged : MonoBehaviour
    {
        public void NotifyValueChange(float newValue)
        {
            float roundedValue = (float)Math.Round(newValue, 2);
            EventManager.ButtonClickEvent.OnSliderValueChanged.RaiseEvent(roundedValue);
        }
    }
}

