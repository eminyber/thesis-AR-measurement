using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.HoldButtonHandlers
{
    public class HoldToGenerateMeshesHandler : MonoBehaviour, IHoldButtonHandler
    {
        public void OnButtonDown()
        {
            EventManager.AppEvent.EnableMeshGeneration.RaiseEvent();
        }

        public void OnButtonUp()
        {
            EventManager.AppEvent.DisableMeshGeneration.RaiseEvent();
            EventManager.AppEvent.RequestGeneratedMeshes.RaiseEvent();
        }
    }
}

