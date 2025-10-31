using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.ButtonClickHandlers
{
    public class ToggleContinuousMeasurements : MonoBehaviour, IButtonClickHandler
    {
       public void OnButtonClick()
        {
            EventManager.ButtonClickEvent.ToggleContinuousMeasurementMode.RaiseEvent();
        }
    }
}

