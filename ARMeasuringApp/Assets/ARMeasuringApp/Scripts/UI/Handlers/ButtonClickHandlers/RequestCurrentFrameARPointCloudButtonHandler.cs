using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.ButtonClickHandlers
{
    public class RequestCurrentFrameARPointCloudButtonHandler : MonoBehaviour, IButtonClickHandler
    {
        public void OnButtonClick()
        {
            EventManager.AppEvent.RequestCurrentFrameARPointCloud.RaiseEvent();
        }
    }
}

