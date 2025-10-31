using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Events.Eventargs;
using ARMeasurementApp.Scripts.Util.Enums;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;

namespace ARMeasurementApp.Scripts.Systems
{
    public class AnchorPointMeasurementSystem: MonoBehaviour
    {
        [SerializeField] Camera _arCamera;
        [SerializeField] LineRenderer _lineRenderPrefab;

        private LineRenderer _distanceLineRenderer;

        private LineRenderController _lineRendererController = new LineRenderController();
        private SelectedObjectTracker<ARPlane> _selectedARPlaneTracker = new SelectedObjectTracker<ARPlane>();
        private SelectedObjectTracker<Pose> _selectedMeasurementPointPose = new SelectedObjectTracker<Pose>();
        private PlaneVisualManager _planeVisualManager = new PlaneVisualManager();

        private bool _isPlaneCenterSelectedAsAnchorPoint = false;

        private AnchorPointDistanceCalculationMode _currentAnchorPointDistanceCalculationMode = AnchorPointDistanceCalculationMode.DirectDistance;
        private int _nrOfAnchorPointDistanceCalculationModes = Enum.GetValues(typeof(AnchorPointDistanceCalculationMode)).Length;

        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedWithinARPlane.AddListener(HandleUserTappedAPlane);

            EventManager.AppEvent.OnARPlaneRemoved.AddListener(HandleSystemRemovedARPlane);

            EventManager.ButtonClickEvent.ToggleAnchorPointUsed.AddListener(UpdateAnchorPointUsed);
            EventManager.ButtonClickEvent.ChangeAnchorPointCalculationMode.AddListener(UpdateCurrentDistanceCalculationMode);
            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
        }

        void OnDisable()
        {
            EventManager.TouchEvent.UserTappedWithinARPlane.RemoveListener(HandleUserTappedAPlane);

            EventManager.AppEvent.OnARPlaneRemoved.RemoveListener(HandleSystemRemovedARPlane);

            EventManager.ButtonClickEvent.ToggleAnchorPointUsed.RemoveListener(UpdateAnchorPointUsed);
            EventManager.ButtonClickEvent.ChangeAnchorPointCalculationMode.RemoveListener(UpdateCurrentDistanceCalculationMode);
            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);

            DisableARPlaneChangeTracker();
        }

        void Start()
        {
            if (_arCamera == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in RayCastDistanceMeasurementSystem: _arCamera is null");
                enabled = false;
                return;
            }

            if (_lineRenderPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in RayCastDistanceMeasurementSystem: _lineRenderPrefab is null");
                enabled = false;
                return;
            }

            _distanceLineRenderer = Instantiate(_lineRenderPrefab);

            EnableARPlaneChangeTracker();

            TellUIAnchorPointDistanceCalculationModeChanged();
            TellUIARPlaneCenterAsAnchorPointStateChanged();
        }

        void Update()
        {
            if (_selectedARPlaneTracker.CurrentSelectedObject == null) return;

            ConductDistanceToAnchorPointMeasurement();
        }

        private void HandleUserTappedAPlane(object sender, PlaneAndPoseOnPlaneEventArgs e)
        {
            if (e == null) return;

            var currentSelectedPlane = _selectedARPlaneTracker.CurrentSelectedObject;
            var touchedPlane = e.Plane;
            if (currentSelectedPlane != null)
            {
                DeselectPlane(currentSelectedPlane);
                _selectedMeasurementPointPose.ClearCurrentSelectedObject();
                ClearVisuals();

                if (touchedPlane == currentSelectedPlane)
                    return;
            }

            SelectPlane(touchedPlane);
            _selectedMeasurementPointPose.SetCurrentSelectedObject(e.Pose);
        }

        private void HandleSystemRemovedARPlane(ARPlane removedPlane)
        {
            if (removedPlane == null || !removedPlane.Equals(_selectedARPlaneTracker.CurrentSelectedObject)) return;

            EventManager.AppEvent.LogWarning.RaiseEvent("Warning in RaycastDistanceMeasurementSystem -> HandleRemovedARPlaned: The current selected plane got removed by the system");

            DeselectPlane(removedPlane);
            _selectedMeasurementPointPose.ClearCurrentSelectedObject();
            ClearVisuals();
        }

        private void UpdateAnchorPointUsed()
        {
            _isPlaneCenterSelectedAsAnchorPoint = !_isPlaneCenterSelectedAsAnchorPoint;
            TellUIARPlaneCenterAsAnchorPointStateChanged();
        }

        private void UpdateCurrentDistanceCalculationMode()
        {
            int newRaycastDistanceCalculationMode = ((int)_currentAnchorPointDistanceCalculationMode + 1) % _nrOfAnchorPointDistanceCalculationModes;
            _currentAnchorPointDistanceCalculationMode = (AnchorPointDistanceCalculationMode)newRaycastDistanceCalculationMode;

            TellUIAnchorPointDistanceCalculationModeChanged();
        }

        private void ResetSystem()
        {
            if (_selectedARPlaneTracker.CurrentSelectedObject == null) return;

            DeselectPlane(_selectedARPlaneTracker.CurrentSelectedObject);
            _selectedMeasurementPointPose.ClearCurrentSelectedObject();
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

        private void ConductDistanceToAnchorPointMeasurement()
        {
            if (_selectedARPlaneTracker.CurrentSelectedObject == null) return;

            var measurementPointPosition = GetMeasurementPointPosition();
            var startPosition = _arCamera.transform.position;
            
            if (_currentAnchorPointDistanceCalculationMode == AnchorPointDistanceCalculationMode.HorizontalDistance)
            {
                startPosition.y = measurementPointPosition.y;
            }

            if (_currentAnchorPointDistanceCalculationMode == AnchorPointDistanceCalculationMode.VerticalDistance)
            {
                startPosition.x = measurementPointPosition.x;
                startPosition.z = measurementPointPosition.z;
            }
           
            var distance = Vector3.Distance(startPosition, measurementPointPosition);

            DisplayDistanceInformation(distance, startPosition, measurementPointPosition);
        }

        private void DisplayDistanceInformation(float distance, Vector3 startPosition, Vector3 endPosition)
        {
            _lineRendererController.DrawALine(_distanceLineRenderer, startPosition, endPosition);

            TellUIToUpdateInfoText($"Current distance to position: {distance}m");
        }

        private void ClearVisuals()
        {
            _lineRendererController.ClearLines(_distanceLineRenderer);

            TellUIToUpdateInfoText(string.Empty);
        }

        private Vector3 GetMeasurementPointPosition()
        {
            return _isPlaneCenterSelectedAsAnchorPoint ? _selectedARPlaneTracker.CurrentSelectedObject.center : _selectedMeasurementPointPose.CurrentSelectedObject.position;
        }

        private void TellUIToUpdateInfoText(string str)
        {
            EventManager.UIEvent.UpdateInfoText.RaiseEvent(str);
        }

        private void TellUIARPlaneCenterAsAnchorPointStateChanged()
        {
            EventManager.UIEvent.UseARPlaneCenterAsAnchorPointStateChanged.RaiseEvent(_isPlaneCenterSelectedAsAnchorPoint);
        }

        private void TellUIAnchorPointDistanceCalculationModeChanged()
        {
            EventManager.UIEvent.AnchorPointDistanceCalculationModeChanged.RaiseEvent(_currentAnchorPointDistanceCalculationMode);
        }

        private void EnableARPlaneChangeTracker()
        {
            EventManager.AppEvent.EnableARPlaneChangeTracking.RaiseEvent();
        }

        private void DisableARPlaneChangeTracker()
        {
            EventManager.AppEvent.DisableARPlaneChangeTracking.RaiseEvent();
        }
    }
}

