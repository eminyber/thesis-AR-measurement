using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Handlers
{
    public class ARPlaneChangeTracker : MonoBehaviour 
    {
        [SerializeField] ARPlaneManager _arPlaneManager;

        private bool _isTrackingPlaneChanges = false;

        void OnEnable()
        {
            EventManager.AppEvent.EnableARPlaneChangeTracking.AddListener(EnablePlaneChangeTracking);
            EventManager.AppEvent.DisableARPlaneChangeTracking.AddListener(DisablePlaneChangeTracking);
        }

        void OnDisable()
        {
            EventManager.AppEvent.EnableARPlaneChangeTracking.RemoveListener(EnablePlaneChangeTracking);
            EventManager.AppEvent.DisableARPlaneChangeTracking.RemoveListener(DisablePlaneChangeTracking);

            if (_isTrackingPlaneChanges)
                DisablePlaneChangeTracking();
        }

        void Start()
        {
            if (_arPlaneManager != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in ARPlaneChangeTracker: _arPlaneManager is null");
            enabled = false;
        }

        private void EnablePlaneChangeTracking()
        {
            if (_isTrackingPlaneChanges) return;

            _arPlaneManager.trackablesChanged.AddListener(OnTrackablesChanged);
            _isTrackingPlaneChanges = true;
        }

        private void DisablePlaneChangeTracking()
        {
            if (!_isTrackingPlaneChanges) return;

            _arPlaneManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
            _isTrackingPlaneChanges = false;
        }

        private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
        {
            foreach (var plane in changes.updated)
            {
               EventManager.AppEvent.OnARPlaneUpdated.RaiseEvent(plane);
            }

            foreach (var kvPlane in changes.removed)
            {
                EventManager.AppEvent.OnARPlaneRemoved.RaiseEvent(kvPlane.Value);
            }
        }
    }
}