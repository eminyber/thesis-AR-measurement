using ARMeasurementApp.Scripts.Util.Enums;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Events.AppEvents
{
    public class AppEvent
    {
        // Scene and Modes events
        public readonly Event<ARMeasurementAppScene> SwitchScene = new();

        // Plane Change Tracking events
        public readonly Event EnableARPlaneChangeTracking = new();
        public readonly Event DisableARPlaneChangeTracking = new();

        public readonly Event<ARPlane> OnARPlaneUpdated = new();
        public readonly Event<ARPlane> OnARPlaneRemoved = new();

        // ARAnchor Change Events
        public readonly Event<ARAnchor> OnARAnchorAdded = new();
        public readonly Event<ARAnchor> OnARAnchorUpdated = new();
        public readonly Event<ARAnchor> OnARAnchorRemoved = new();
        
        // Manage Point Cloud Event
        public readonly Event EnableARPointCloudGeneration = new();
        public readonly Event DisableARPointCloudGeneration = new();

        public readonly Event StoreCurrentFrameARPointCloud = new();
        public readonly Event ClearStoredARPointClouds = new();

        public readonly Event RequestCurrentFrameARPointCloud = new();
        public readonly Event RequestStoredARPointClouds = new();

        public readonly Event<List<ARPointCloud>> OnARPointCloudsSent = new();

        // Manage Meshing Events
        public readonly Event EnableMeshGeneration = new();
        public readonly Event DisableMeshGeneration = new();

        public readonly Event RequestGeneratedMeshes = new();
        public readonly Event<List<MeshFilter>> OnMeshesSent = new();

        public readonly Event DestroyAllGeneratedMeshes = new();

        // Loggin Events
        public readonly Event<string> Log = new();
        public readonly Event<string> LogWarning = new();
        public readonly Event<string> LogError = new();
        public readonly Event ClearLog = new();
    }
}


