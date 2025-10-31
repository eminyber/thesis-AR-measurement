using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.ButtonClickHandlers
{
    public class ToggleConfidenceValueFilteringButtonHandler : MonoBehaviour, IButtonClickHandler
    {
        
        public void OnButtonClick()
        {
            EventManager.ButtonClickEvent.ToggleConfidenceValueFiltering.RaiseEvent();
        }
    }
}

