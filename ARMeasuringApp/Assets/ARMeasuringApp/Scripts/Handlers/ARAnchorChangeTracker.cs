using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Handlers
{
    public class ARAnchorChangeTracker : MonoBehaviour
    {
        [SerializeField] ARAnchorManager _arAnchorManager;

        private bool _isTracking = false;

        void OnDisable()
        {
            if (_isTracking)
            {
                _arAnchorManager.trackablesChanged.RemoveListener(OnARAnchorsChanged);
                _isTracking = false;
            }
        }

        void Start()
        {
            if (_arAnchorManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in ARAnchorChangeTracker: _arAnchorManager is null");
                enabled = false;
                return;
            }

            _arAnchorManager.trackablesChanged.AddListener(OnARAnchorsChanged);
            _isTracking=true;
        }

        private void OnARAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> changes)
        {
            foreach (var anchor in changes.added)
            {
                EventManager.AppEvent.OnARAnchorAdded.RaiseEvent(anchor);
            }

            foreach (var anchor in changes.updated)
            {
                EventManager.AppEvent.OnARAnchorUpdated.RaiseEvent(anchor);
            }

            foreach (var anchor in changes.removed)
            {
                EventManager.AppEvent.OnARAnchorRemoved.RaiseEvent(anchor.Value);
            }
        }
    }
}

