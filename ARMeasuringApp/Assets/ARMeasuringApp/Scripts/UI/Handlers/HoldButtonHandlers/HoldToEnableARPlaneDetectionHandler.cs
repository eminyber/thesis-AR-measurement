using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.HoldButtonHandlers
{
    public class HoldToEnableARPlaneDetectionHandler : MonoBehaviour, IHoldButtonHandler 
    {
        public void OnButtonDown()
        {
            EventManager.ButtonClickEvent.EnableARPlaneDetection.RaiseEvent();
        }

        public void OnButtonUp()
        {
            EventManager.ButtonClickEvent.DisableARPlaneDetection.RaiseEvent();
        }
    }
}

