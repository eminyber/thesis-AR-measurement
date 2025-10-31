using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Objects;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Util;

using System;
using System.Collections.Generic;
using g4;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

namespace ARMeasurementApp.Scripts.Systems
{
    public class AAMBBWithPointCloudDataMeasurementSystem : MonoBehaviour
    {
        [SerializeField] ObjectOutlineManager _objectOutlineManager;
        [SerializeField] TextMeshProsManager _labelManager;
        [SerializeField] BoxRenderController _boxRenderController;

        private ARPointCloudController _pointCloudController = new ARPointCloudController();

        private AABB _axisAlignedBoundingBox;

        private bool _filterByMinimumConfidenceValue = false;
        private float _minimumConfidenceValue = 0f;

        private const float OUTLINE_LEVEL_OFFSET = 0.01f;

        private const float LABEL_UPSCALAR = 0.025f;

        private const float MINIMUM_TEXT_SIZE = 0.30f;
        private const float MAXIMUM_TEXT_SIZE = 0.75f;

        private const int FLOAT_ROUND_PRECISION = 5;

        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.AddListener(AddObjectOutlinePoint);

            EventManager.AppEvent.OnARPointCloudsSent.AddListener(ContructAABBFromPointCloudData);

            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
            EventManager.ButtonClickEvent.OnSliderValueChanged.AddListener(UpdateMinimumConfidenceValue);
            EventManager.ButtonClickEvent.ToggleConfidenceValueFiltering.AddListener(ToggleConfidenceValueFiltering);
        }

        void OnDisable() 
        {
            EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.RemoveListener(AddObjectOutlinePoint);

            EventManager.AppEvent.OnARPointCloudsSent.RemoveListener(ContructAABBFromPointCloudData);

            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);
            EventManager.ButtonClickEvent.OnSliderValueChanged.RemoveListener(UpdateMinimumConfidenceValue);
            EventManager.ButtonClickEvent.ToggleConfidenceValueFiltering.RemoveListener(ToggleConfidenceValueFiltering);
        }

        void Start()
        {
            if (_objectOutlineManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithPointCloudDataMeasurementSystem: _outlineObjectManager is null");
                enabled = false;
                return;
            }

            if (_labelManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithPointCloudDataMeasurementSystem: _labelManager is null");
                enabled = false;
                return;
            }

            if (_boxRenderController == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithPointCloudDataMeasurementSystem: _boxRenderController is null");
                enabled = false;
                return;
            }

            TellUIFilterByMinimumConfidenceStateChanged();
        }

        private void AddObjectOutlinePoint(Pose pose)
        {
            if (pose == null || _objectOutlineManager.IsOutlineCompleted()) return;

            GameObject newOutlinePoint = _objectOutlineManager.AddOutlinePoint(pose);
            if (newOutlinePoint == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithPointCloudDataMeasurementSystem -> AddObjectOutlinePoint: A new outline point could not be initialized");
                return;
            }

            if (_objectOutlineManager.OutlineVerticesCount == 4) 
            {
                _objectOutlineManager.ConstructObjectOutline();
                TellUIToUpdateScanButtonVisibility(_objectOutlineManager.IsOutlineCompleted());
            }
        }

        private void ContructAABBFromPointCloudData(List<ARPointCloud> gatheredPointClouds)
        {
            if (gatheredPointClouds == null) return;

            EventManager.AppEvent.Log.RaiseEvent("Nr of pointclouds in construct AAMBB: " + gatheredPointClouds.Count.ToString());

            var filteredPositions = FilterCloudPointPositions(gatheredPointClouds);

            var outlineCenterPosition = _objectOutlineManager.GetOutlineCenter();

            ClearObjectOutline();

            AxisAlignedBox3d axisAlignedBox = new AxisAlignedBox3d((Vector3d) outlineCenterPosition);
            foreach (Vector3 filteredPosition in filteredPositions)
            {
                axisAlignedBox.Contain((Vector3d)filteredPosition);
            }

            _axisAlignedBoundingBox = new AABB(axisAlignedBox);

            var cornerPositions = _axisAlignedBoundingBox.GetCornerPositions();
            _boxRenderController.RenderBox(cornerPositions);

            var dimensionInfo = _axisAlignedBoundingBox.GetBoxDimensionInfo();
            for (int i = 0; i < dimensionInfo.Count; i++)
            {
                var distance = dimensionInfo[i].Distance;
                var sourcePosition = dimensionInfo[i].SourcePosition;
                var destinationPosition = dimensionInfo[i].DestinationPosition;

                // Move the height labels forward to prevent them from being clipped by the rendered lines
                if (i > dimensionInfo.Count - 5)
                {
                    sourcePosition.z -= 0.025f;
                    destinationPosition.z -= 0.025f;
                }

                CreateNewMeasurementLabel(distance, sourcePosition, destinationPosition);  
            }

            ClearStoredARPointClouds();
        }

        private void ResetSystem()
        {
            ClearObjectOutline();
            _labelManager.DeleteAllTextMeshes();
            _boxRenderController.EraseRenderedBox();
        }

        private void UpdateMinimumConfidenceValue(float newValue)
        {
            _minimumConfidenceValue = newValue;
        }

        private void ToggleConfidenceValueFiltering()
        {
            _filterByMinimumConfidenceValue = !_filterByMinimumConfidenceValue;
            TellUIFilterByMinimumConfidenceStateChanged();
        }

        private void ClearObjectOutline()
        {
            _objectOutlineManager.ClearObjectOutline();
            TellUIToUpdateScanButtonVisibility(_objectOutlineManager.IsOutlineCompleted());
        }

        private List<Vector3> FilterCloudPointPositions(List<ARPointCloud> pointClouds)
        {
            if (pointClouds == null) return new List<Vector3>();

            List<Vector3> pointCloudPositions;
            if (_filterByMinimumConfidenceValue)
            {
                pointCloudPositions = _pointCloudController.GetFeaturePointsPositionsAtOrAboveConfidenceValue(pointClouds, _minimumConfidenceValue);
            }
            else
            {
                pointCloudPositions = _pointCloudController.GetFeaturePointsPositions(pointClouds);
            }

            //EventManager.AppEvent.Log.RaiseEvent("Nr of PointCloudPositions in construct AAMBB: " + pointCloudPositions.Count.ToString());

            //Filter out positions outside off the marked area
            var objectOutlineVerticiesPositions = _objectOutlineManager.GetOutlineVerticesIn2D();
            var filteredPointCloudPositions = PositionFilteringUtils.RemovePositionsOutsideMarkedArea(pointCloudPositions, objectOutlineVerticiesPositions); 

            //Filter out positions on the floor
            float objectOutlineHeight = _objectOutlineManager.GetOutlineHeightInWorld();
            filteredPointCloudPositions = PositionFilteringUtils.RemovePositionsUnderLevel(filteredPointCloudPositions, objectOutlineHeight, OUTLINE_LEVEL_OFFSET, Vector3.up);;

            return filteredPointCloudPositions;
        }

        private void CreateNewMeasurementLabel(double distance, Vector3 sourcePosition, Vector3 destinationPosition)
        {
            var roundedDistance = (float)Math.Round(distance, FLOAT_ROUND_PRECISION);

            var labelPosition = TextAlignmentUtils.CalculateLabelPositionAboveMidpointInWorld(sourcePosition, destinationPosition, LABEL_UPSCALAR);
            var labelRotation = TextAlignmentUtils.CalculateLabelRotationInWorld(sourcePosition, destinationPosition);

            TextMeshPro newLabel = _labelManager.CreateNewTextMesh(labelPosition, labelRotation);
            if (newLabel == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithPointCloudDataMeasurementSystem -> CreateMeasurementLabel: A new TextMesh could not be created");
                return;
            }

            newLabel.fontSize = Math.Clamp(newLabel.fontSize * roundedDistance, MINIMUM_TEXT_SIZE, MAXIMUM_TEXT_SIZE);
            newLabel.text = roundedDistance.ToString() + "m";
        }

        private void TellUIToUpdateScanButtonVisibility(bool shouldBeVisible)
        {
            EventManager.UIEvent.UpdateScanButtonVisibility.RaiseEvent(shouldBeVisible);
        }

        private void ClearStoredARPointClouds()
        {
            EventManager.AppEvent.ClearStoredARPointClouds.RaiseEvent();
        }

        private void TellUIFilterByMinimumConfidenceStateChanged()
        {
            EventManager.UIEvent.ConfidenceValueFilteringStateChanged.RaiseEvent(_filterByMinimumConfidenceValue);
        }
    }
}

