using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Events.Eventargs;
using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Util;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Systems
{
    public class PlaneExtensionMeasurementSystem : MonoBehaviour
    {
        [SerializeField] GameObject _axisIndicatorPrefab;

        [SerializeField] LineRenderer _widthLineRenderPrefab;
        [SerializeField] LineRenderer _heightLineRenderPrefab;

        [SerializeField] string _widthLineColor = "White";
        [SerializeField] string _heightLineColor = "Red";

        private GameObject _axisIndicator;

        private LineRenderer _widthLineRenderer;
        private LineRenderer _heightLineRenderer;

        private LineRenderController _lineRendererController = new LineRenderController();
        private SelectedObjectTracker<ARPlane> _selectedARPlaneTracker = new SelectedObjectTracker<ARPlane>();
        private PlaneVisualManager _planeVisualManager = new PlaneVisualManager();
        private ARPointCloudController _pointCloudController = new ARPointCloudController();

        private float _planeLevelBoundary = 0.01f;

        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedWithinARPlane.AddListener(HandleUserTouchedAPlane);

            EventManager.AppEvent.OnARPointCloudsSent.AddListener(ConductPlaneExtensionMeasurement);
            EventManager.AppEvent.OnARPlaneRemoved.AddListener(HandleSystemRemovedPlane);

            EventManager.ButtonClickEvent.RotateObject.AddListener(RotateAxisIndicator);
            EventManager.ButtonClickEvent.OnSliderValueChanged.AddListener(UpdatePlaneLevelBoundary);
            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
        }

        void OnDisable()
        {
            EventManager.TouchEvent.UserTappedWithinARPlane.RemoveListener(HandleUserTouchedAPlane);

            EventManager.AppEvent.OnARPointCloudsSent.RemoveListener(ConductPlaneExtensionMeasurement);
            EventManager.AppEvent.OnARPlaneRemoved.RemoveListener(HandleSystemRemovedPlane);

            EventManager.ButtonClickEvent.RotateObject.RemoveListener(RotateAxisIndicator);
            EventManager.ButtonClickEvent.OnSliderValueChanged.RemoveListener(UpdatePlaneLevelBoundary);
            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);

            DisableARPlaneChangeTracker();
        }

        void Start()
        {
            if (_axisIndicatorPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneExtensionMeasurementSystem: _axisIndicatorPrefab is null");
                enabled = false;
                return;
            }

            if (_widthLineRenderPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneExtensionMeasurementSystem: _widthLineRenderPrefab is null");
                enabled = false;
                return;
            }

            if (_heightLineRenderPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneExtensionMeasurementSystem: _heightLineRenderPrefab is null");
                enabled = false;
                return;
            }

            _widthLineRenderer  = Instantiate(_widthLineRenderPrefab);
            _heightLineRenderer = Instantiate(_heightLineRenderPrefab);

            EnableARPlaneChangeTracker();
        }

        private void HandleUserTouchedAPlane(object sender, PlaneAndPoseOnPlaneEventArgs e)
        {
            if (e == null) return;

            var currentSelectedPlane = _selectedARPlaneTracker.CurrentSelectedObject;
            var touchedPlane = e.Plane;
            if (currentSelectedPlane != null)
            {
                DeselectPlane(currentSelectedPlane);
                DeactivatePlanextensionMeasurementMode();
                ClearVisuals();

                if (touchedPlane == currentSelectedPlane)
                    return;
            }

            SelectPlane(touchedPlane);
            ActivatePlaneExtensionMeasurementMode();
        }

        private void ConductPlaneExtensionMeasurement(List<ARPointCloud> pointClouds)
        {
            if (pointClouds == null) return;

            var currentSelectedPlane = _selectedARPlaneTracker.CurrentSelectedObject;

            var featurePointPositions = _pointCloudController.GetFeaturePointsPositions(pointClouds);

            var filteredFeaturePointPositions = PositionFilteringUtils.RemovePositionsNotOnPlane(featurePointPositions, currentSelectedPlane.center, currentSelectedPlane.normal.normalized, _planeLevelBoundary);
            if (filteredFeaturePointPositions.Count == 0)
            {
                EventManager.AppEvent.ClearLog.RaiseEvent();
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in PlaneExtensionMeasurementSystem -> ConductPlaneExtensionMeasurement: No feature points found near the level of the selected plane");
            }

            //Add the four world positions that make up the start and end of the selected ARPlane's height and width, if no feature points are found
            //at the same level of the selected plane, then these four points will be used as default to decide the width and length respectively. 
            Vector3 selectedPlaneWidthStartWorldPosition = currentSelectedPlane.transform.position - (currentSelectedPlane.extents.x * currentSelectedPlane.transform.right);
            Vector3 selectedPlaneWidthEndWorldPosition = currentSelectedPlane.transform.position + (currentSelectedPlane.extents.x * currentSelectedPlane.transform.right);

            Vector3 selectedPlaneLengthStartWorldposition = currentSelectedPlane.transform.position - (currentSelectedPlane.extents.y * currentSelectedPlane.transform.forward);
            Vector3 selectedPlaneLengthEndWorldposition = currentSelectedPlane.transform.position + (currentSelectedPlane.extents.y * currentSelectedPlane.transform.forward);

            filteredFeaturePointPositions.Add(selectedPlaneWidthStartWorldPosition);
            filteredFeaturePointPositions.Add(selectedPlaneWidthEndWorldPosition);
            filteredFeaturePointPositions.Add(selectedPlaneLengthStartWorldposition);
            filteredFeaturePointPositions.Add(selectedPlaneLengthEndWorldposition);

            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minZ = float.MaxValue;
            var maxZ = float.MinValue;
            foreach (Vector3 worldPosition in filteredFeaturePointPositions)
            {
                Vector3 localPoint = _axisIndicator.transform.InverseTransformPoint(worldPosition);
                var localX = localPoint.x;
                var localZ = localPoint.z;

                minX = Mathf.Min(minX, localX);
                maxX = Mathf.Max(maxX, localX);
                minZ = Mathf.Min(minZ, localZ);
                maxZ = Mathf.Max(maxZ, localZ);
            }

            Vector3 widthStartWorldPosition = _axisIndicator.transform.TransformPoint(new Vector3(minX, 0, 0));
            Vector3 widthEndWorldPosition = _axisIndicator.transform.TransformPoint(new Vector3(maxX, 0, 0));

            Vector3 lengthStartWorldPosition = _axisIndicator.transform.TransformPoint(new Vector3(0, 0, minZ));
            Vector3 lengthEndWorldPosition = _axisIndicator.transform.TransformPoint(new Vector3(0, 0, maxZ));

            DisplaySizeInformation(widthStartWorldPosition, widthEndWorldPosition, lengthStartWorldPosition, lengthEndWorldPosition);
        }

        private void HandleSystemRemovedPlane(ARPlane removedPlane)
        {
            if (removedPlane == null || !removedPlane.Equals(_selectedARPlaneTracker.CurrentSelectedObject)) return;

            EventManager.AppEvent.LogWarning.RaiseEvent("Warning in PlaneExtensionMeasurementSystem -> HandleRemovedARPlaned: The current selected plane got removed by the system");

            DeselectPlane(removedPlane);
            DeactivatePlanextensionMeasurementMode();
            ClearVisuals();
        }

        private void RotateAxisIndicator(Vector3 eulerRotation)
        {
            if (_axisIndicator == null) return;

            _axisIndicator.transform.Rotate(eulerRotation);
        }

        private void UpdatePlaneLevelBoundary(float newValue)
        {
            _planeLevelBoundary = newValue;
        }

        private void ResetSystem()
        {
            EventManager.AppEvent.ClearLog.RaiseEvent();

            if (_selectedARPlaneTracker.CurrentSelectedObject == null) return;

            DeselectPlane(_selectedARPlaneTracker.CurrentSelectedObject);
            DeactivatePlanextensionMeasurementMode();
            ClearVisuals();
        }

        private void SelectPlane(ARPlane plane)
        {
            if (plane == null) return;

            _selectedARPlaneTracker.SetCurrentSelectedObject(plane);
            _planeVisualManager.ApplySelectedStateVisual(plane);
        }

        private void DeselectPlane(ARPlane plane)
        {
            if (plane == null) return;

            _planeVisualManager.RemoveSelectedStateVisual(plane);
            _selectedARPlaneTracker.ClearCurrentSelectedObject();
        }

        private void ActivatePlaneExtensionMeasurementMode()
        {
            var currentSelectedPlane = _selectedARPlaneTracker.CurrentSelectedObject;
            _axisIndicator = Instantiate(_axisIndicatorPrefab, currentSelectedPlane.transform.position, currentSelectedPlane.transform.rotation);

            EnablePointCloudGeneration();

            SetPlaneExtensionModeUIVisibility(true);

            TellUIInfoTextToUpdate("Width = along red axis and height = along blue axis");
        }

        private void DeactivatePlanextensionMeasurementMode()
        {
            Destroy(_axisIndicator);

            DisablePointCloudGeneration();

            SetPlaneExtensionModeUIVisibility(false);
        }

        private void DisplaySizeInformation(Vector3 widthStartPosition, Vector3 widthEndPosition, Vector3 lengthStartPosition, Vector3 lengthEndPosition)
        {
            float width = Vector3.Distance(widthStartPosition, widthEndPosition);
            float length = Vector3.Distance(lengthStartPosition, lengthEndPosition);

            _lineRendererController.DrawALine(_widthLineRenderer, widthStartPosition, widthEndPosition);
            _lineRendererController.DrawALine(_heightLineRenderer, lengthStartPosition, lengthEndPosition);

            TellUIInfoTextToUpdate($"Width ({_widthLineColor} line): {width}m Length: ({_heightLineColor} line): {length}m");
        }

        private void ClearVisuals()
        {
            _lineRendererController.ClearLines(_widthLineRenderer);
            _lineRendererController.ClearLines(_heightLineRenderer);

            TellUIInfoTextToUpdate(string.Empty);
        }

        private void TellUIInfoTextToUpdate(string str)
        {
            EventManager.UIEvent.UpdateInfoText.RaiseEvent(str);
        }

        private void EnableARPlaneChangeTracker()
        {
            EventManager.AppEvent.EnableARPlaneChangeTracking.RaiseEvent();
        }

        private void DisableARPlaneChangeTracker()
        {
            EventManager.AppEvent.DisableARPlaneChangeTracking.RaiseEvent();
        }

        private void EnablePointCloudGeneration()
        {
            EventManager.AppEvent.EnableARPointCloudGeneration.RaiseEvent();
        }

        private void DisablePointCloudGeneration()
        {
            EventManager.AppEvent.DisableARPointCloudGeneration.RaiseEvent();
        }

        private void SetPlaneExtensionModeUIVisibility(bool shouldBeVisible)
        {
            EventManager.UIEvent.UpdateExtensionModeUIVisibility.RaiseEvent(shouldBeVisible);
        }
    }
}
