using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Events;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Managers
{
    public class PointCloudManager : MonoBehaviour
    {
        [SerializeField] ARPointCloudManager _arPointCloudManager;

        private List<ARPointCloud> _storedPointClouds = new List<ARPointCloud>();

        void OnEnable()
        {
            EventManager.AppEvent.RequestCurrentFrameARPointCloud.AddListener(SendCurrentFrameARPointCloud);
            EventManager.AppEvent.RequestStoredARPointClouds.AddListener(SendStoredARPointClouds);

            EventManager.AppEvent.StoreCurrentFrameARPointCloud.AddListener(OnStoreCurrentFrameARPointCloud);

            EventManager.AppEvent.EnableARPointCloudGeneration.AddListener(OnEnableARpointCloudGeneration);
            EventManager.AppEvent.DisableARPointCloudGeneration.AddListener(OnDisableARPointCloudGeneration);

            EventManager.AppEvent.ClearStoredARPointClouds.AddListener(OnClearStoredPointClouds);

            if (_arPointCloudManager != null )
                _arPointCloudManager.enabled = false;
        }

        void OnDisable()
        {
            EventManager.AppEvent.RequestCurrentFrameARPointCloud.RemoveListener(SendCurrentFrameARPointCloud);
            EventManager.AppEvent.RequestStoredARPointClouds.RemoveListener(SendStoredARPointClouds);

            EventManager.AppEvent.StoreCurrentFrameARPointCloud.RemoveListener(OnStoreCurrentFrameARPointCloud);

            EventManager.AppEvent.EnableARPointCloudGeneration.RemoveListener(OnEnableARpointCloudGeneration);
            EventManager.AppEvent.DisableARPointCloudGeneration.RemoveListener(OnDisableARPointCloudGeneration);

            EventManager.AppEvent.ClearStoredARPointClouds.RemoveListener(OnClearStoredPointClouds);
        }

        void Start()
        {
            if (_arPointCloudManager != null) return;
            
            EventManager.AppEvent.LogError.RaiseEvent("Error in PointCloudManager: _arPointCloudManager is null");
            enabled = false;
        }

        private void SendCurrentFrameARPointCloud()
        {
            var pointClouds = new List<ARPointCloud>();
            foreach (ARPointCloud pointCloud in _arPointCloudManager.trackables)
            {
                pointClouds.Add(pointCloud);
            }

            EventManager.AppEvent.OnARPointCloudsSent.RaiseEvent(pointClouds);
        }

        private void SendStoredARPointClouds()
        {
            EventManager.AppEvent.OnARPointCloudsSent.RaiseEvent(_storedPointClouds);
        }

        private void OnStoreCurrentFrameARPointCloud()
        {
            foreach (ARPointCloud pointCloud in _arPointCloudManager.trackables)
            {
                _storedPointClouds.Add(pointCloud);
            }
        }

        private void OnEnableARpointCloudGeneration()
        {
            _arPointCloudManager.enabled = true;
        }

        private void OnDisableARPointCloudGeneration()
        {
            _arPointCloudManager.enabled = false;
        }

        private void OnClearStoredPointClouds()
        {
            _storedPointClouds.Clear();
        }
    }
}

