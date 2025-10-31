using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.HoldButtonHandlers
{
    public class HoldToGatherARPointCloudsHandler : MonoBehaviour, IHoldButtonHandler
    {
        private bool _isBeingHeldDown = false;

        public void OnButtonDown()
        {
            _isBeingHeldDown = true;

            EventManager.AppEvent.EnableARPointCloudGeneration.RaiseEvent();
        }

        public void OnButtonUp()
        {
            _isBeingHeldDown = false;

            EventManager.AppEvent.DisableARPointCloudGeneration.RaiseEvent();
            EventManager.AppEvent.RequestStoredARPointClouds.RaiseEvent();
        }
        void LateUpdate()
        {
            if (_isBeingHeldDown)
                EventManager.AppEvent.StoreCurrentFrameARPointCloud.RaiseEvent();
        }
    }
}