using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Events.Eventargs;
using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Services;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Systems 
{
    public class PlaneSelectionMeasurementSystem : MonoBehaviour
    {
        [SerializeField] LineRenderer _widthLineRenderPrefab;
        [SerializeField] LineRenderer _heightLineRenderPrefab;

        [SerializeField] string widthLineColor  = "White";
        [SerializeField] string heightLineColor = "Red";

        private LineRenderer _widthLineRenderer;
        private LineRenderer _heightLineRenderer;

        private LineRenderController _lineRendererController = new LineRenderController();
        private SelectedObjectTracker<ARPlane> _selectedARPlaneTracker = new SelectedObjectTracker<ARPlane>();
        private PlaneVisualManager _planeVisualManager = new PlaneVisualManager();
        private ARPlaneInfoProvider _planeInfoProvider = new ARPlaneInfoProvider();

        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedWithinARPlane.AddListener(TogglePlaneMeasurement);

            EventManager.AppEvent.OnARPlaneUpdated.AddListener(HandleSystemUpdatedPlane);
            EventManager.AppEvent.OnARPlaneRemoved.AddListener(HandleSystemRemovedPlane);

            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
        }

        void OnDisable()
        {
            EventManager.TouchEvent.UserTappedWithinARPlane.RemoveListener(TogglePlaneMeasurement);

            EventManager.AppEvent.OnARPlaneUpdated.RemoveListener(HandleSystemUpdatedPlane);
            EventManager.AppEvent.OnARPlaneRemoved.RemoveListener(HandleSystemRemovedPlane);

            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);

            DisableARPlaneChangeTracker();
        }

        void Start()
        {
            if (_widthLineRenderPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneSelectionMeasurementSystem: _widthLineRenderPrefab is null");
                enabled = false;
                return;
            }

            if (_heightLineRenderPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PlaneSelectionMeasurementSystem: _heightLineRenderPrefab is null");
                enabled = false;
                return;
            }

            _widthLineRenderer  = Instantiate(_widthLineRenderPrefab);
            _heightLineRenderer = Instantiate(_heightLineRenderPrefab);

            EnableARPlaneChangeTracker();
        }

        private void TogglePlaneMeasurement(object sender, PlaneAndPoseOnPlaneEventArgs e)
        {
            if (e.Plane == null) return;

            if (e.Plane.Equals(_selectedARPlaneTracker.CurrentSelectedObject))
            {
                StopMeasuringPlane(_selectedARPlaneTracker.CurrentSelectedObject);
            }
            else
            {
                if (_selectedARPlaneTracker.CurrentSelectedObject != null)
                    StopMeasuringPlane(_selectedARPlaneTracker.CurrentSelectedObject);

                StartMeasuringPlane(e.Plane);
            }
        }

        private void HandleSystemUpdatedPlane(ARPlane plane) 
        {
            if (plane == null || !_selectedARPlaneTracker.CurrentSelectedObject.Equals(plane)) return;

            DisplayPlaneDimensionInformation(plane);
        }

        private void HandleSystemRemovedPlane(ARPlane plane)
        {
            if (plane == null || !_selectedARPlaneTracker.CurrentSelectedObject.Equals(plane)) return;

            ClearMeasurementData();
        }

        private void ResetSystem()
        {
            var planeBeingMeasured = _selectedARPlaneTracker.CurrentSelectedObject;
            if (planeBeingMeasured != null)
                StopMeasuringPlane(planeBeingMeasured);
        }

        private void StartMeasuringPlane(ARPlane plane)
        {
            _selectedARPlaneTracker.SetCurrentSelectedObject(plane);
            _planeVisualManager.ApplySelectedStateVisual(plane);

            DisplayPlaneDimensionInformation(plane);
        }

        private void StopMeasuringPlane(ARPlane plane)
        {
            _planeVisualManager.RemoveSelectedStateVisual(plane);
            ClearMeasurementData();
        }

        private void ClearMeasurementData()
        {
            _selectedARPlaneTracker.ClearCurrentSelectedObject();

            _lineRendererController.ClearLines(_widthLineRenderer);
            _lineRendererController.ClearLines(_heightLineRenderer);

            TellUIToUpdateInfoText(string.Empty);
        }

        private void DisplayPlaneDimensionInformation(ARPlane plane)
        {
            if (plane == null) return;

            _lineRendererController.DrawALine(_widthLineRenderer, _planeInfoProvider.GetPlaneWidthStartPosition(plane), _planeInfoProvider.GetPlaneWidthEndPosition(plane));
            _lineRendererController.DrawALine(_heightLineRenderer, _planeInfoProvider.GetPlaneHeightStartPosition(plane), _planeInfoProvider.GetPlaneHeightEndPosition(plane));

            TellUIToUpdateInfoText(_planeInfoProvider.GetPlaneDimensionInfoAsStringWithLineColors(plane, widthLineColor, heightLineColor));
        }

        private void TellUIToUpdateInfoText(string str)
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
    }
}

