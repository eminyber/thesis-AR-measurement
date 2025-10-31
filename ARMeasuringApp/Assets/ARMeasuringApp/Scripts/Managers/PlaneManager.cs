using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Managers
{
    public class PlaneManager : MonoBehaviour
    {
        [SerializeField] ARPlaneManager _arPlaneManager;

        void OnEnable()
        {
            EventManager.ButtonClickEvent.EnableARPlaneDetection.AddListener(OnEnableARPlaneDetection);
            EventManager.ButtonClickEvent.DisableARPlaneDetection.AddListener(OnDisableARPlaneDetection);

            if (_arPlaneManager != null)
                _arPlaneManager.enabled = false;
        }

        void OnDisable()
        {
            EventManager.ButtonClickEvent.EnableARPlaneDetection.RemoveListener(OnEnableARPlaneDetection);
            EventManager.ButtonClickEvent.DisableARPlaneDetection.RemoveListener(OnDisableARPlaneDetection);
        }

        void Start()
        {
            if (_arPlaneManager != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneManager: arPlaneManager is null");
            enabled = false;
        }

        private void OnEnableARPlaneDetection()
        {
            _arPlaneManager.enabled = true;
        }

        private void OnDisableARPlaneDetection()
        {
            _arPlaneManager.enabled = false;
        }
    }
}

