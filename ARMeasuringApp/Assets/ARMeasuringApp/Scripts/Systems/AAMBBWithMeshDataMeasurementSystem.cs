using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Objects;
using ARMeasurementApp.Scripts.Util;

using System;
using System.Collections.Generic;
using g4;

using UnityEngine;
using TMPro;


namespace ARMeasurementApp.Scripts.Systems
{
    public class AAMBBWithMeshDataMeasurementSystem : MonoBehaviour
    {
        [SerializeField] ObjectOutlineManager _objectOutlineManager;
        [SerializeField] TextMeshProsManager _labelManager;
        [SerializeField] BoxRenderController _boxRenderController;

        private MeshFilterController _meshFilterController = new MeshFilterController();

        private AABB _axisAlignedBoundingBox;

        private const float OUTLINE_LEVEL_OFFSET = 0.01f;

        private const float LABEL_UPSCALAR = 0.025f;

        private const float MINIMUM_TEXT_SIZE = 0.30f;
        private const float MAXIMUM_TEXT_SIZE = 0.75f;

        private const int FLOAT_ROUND_PRECISION = 5;
        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.AddListener(AddObjectOutlinePoint);

            EventManager.AppEvent.OnMeshesSent.AddListener(ConstructAABBFromMeshData);

            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
        }

        void OnDisable()
        {
            EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.RemoveListener(AddObjectOutlinePoint);

            EventManager.AppEvent.OnMeshesSent.RemoveListener(ConstructAABBFromMeshData);

            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);
        }

        void Start()
        {
            if (_objectOutlineManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithMeshDataMeasurementSystem: _outlineObjectManager is null");
                enabled = false;
                return;
            }

            if (_labelManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithMeshDataMeasurementSystem: _labelManager is null");
                enabled = false;
                return;
            }

            if (_boxRenderController == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithMeshDataMeasurementSystem: _boxRenderController is null");
                enabled = false;
                return;
            }
        }

        private void AddObjectOutlinePoint(Pose pose)
        {
            if (pose == null || _objectOutlineManager.IsOutlineCompleted()) return;

            GameObject newOutlinePoint = _objectOutlineManager.AddOutlinePoint(pose);
            if (newOutlinePoint == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithMeshDataMeasurementSystem -> AddObjectOutlinePoint: A new outline point was not initialized");
                return;
            }

            if (_objectOutlineManager.OutlineVerticesCount == 4)
            {
                _objectOutlineManager.ConstructObjectOutline();
                TellUIToUpdateScanButtonVisibility(_objectOutlineManager.IsOutlineCompleted());
            }
        }

       
        private void ConstructAABBFromMeshData(List<MeshFilter> meshfilters)
        {
            if (meshfilters == null) return;

            var filteredPositions = FilterMeshPositions(meshfilters);

            var outlineCenterPosition = _objectOutlineManager.GetOutlineCenter();

            ClearObjectOutline();

            AxisAlignedBox3d axisAlignedBox = new AxisAlignedBox3d((Vector3d)outlineCenterPosition);
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

            DestroyAllGeneratedMeshes();
        }

        private void ResetSystem()
        {
            ClearObjectOutline();
            _labelManager.DeleteAllTextMeshes();
            _boxRenderController.EraseRenderedBox();
        }

        private void ClearObjectOutline()
        {
            _objectOutlineManager.ClearObjectOutline();
            TellUIToUpdateScanButtonVisibility(_objectOutlineManager.IsOutlineCompleted());
        }

        private List<Vector3> FilterMeshPositions(List<MeshFilter> meshes)
        {
            if (meshes == null) return new List<Vector3>();

            var verticesPositions = _meshFilterController.GetMeshesVerticePositions(meshes);
            EventManager.AppEvent.Log.RaiseEvent("In filteredMeshPositions total positions: " + verticesPositions.Count.ToString());

            //Filter Out Positions Outside of the marked Area
            var objectOutlineVerticiesPositions = _objectOutlineManager.GetOutlineVerticesIn2D();
            var filteredPositions = PositionFilteringUtils.RemovePositionsOutsideMarkedArea(verticesPositions, objectOutlineVerticiesPositions);
            EventManager.AppEvent.Log.RaiseEvent("Nr of positions after outline filtration: " + filteredPositions.Count.ToString());

            //Filter Out Positions On the floor
            float objectOutlineLevel = _objectOutlineManager.GetOutlineHeightInWorld();
            filteredPositions = PositionFilteringUtils.RemovePositionsUnderLevel(filteredPositions, objectOutlineLevel, OUTLINE_LEVEL_OFFSET, Vector3.up); ;
            EventManager.AppEvent.Log.RaiseEvent("Nr of positions after floor filtration: " + filteredPositions.Count.ToString());

            return filteredPositions;
        }

        private void CreateNewMeasurementLabel(double distance, Vector3 sourcePosition, Vector3 destinationPosition)
        {
            var roundedDistance = (float)Math.Round(distance, FLOAT_ROUND_PRECISION);

            var labelPosition = TextAlignmentUtils.CalculateLabelPositionAboveMidpointInWorld(sourcePosition, destinationPosition, LABEL_UPSCALAR);
            var labelRotation = TextAlignmentUtils.CalculateLabelRotationInWorld(sourcePosition, destinationPosition);

            TextMeshPro newLabel = _labelManager.CreateNewTextMesh(labelPosition, labelRotation);
            if (newLabel == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in AAMBBWithMeshDataMeasurementSystem -> CreateMeasurementLabel: A new TextMesh could not be created");
                return;
            }

            newLabel.fontSize = Math.Clamp(newLabel.fontSize * roundedDistance, MINIMUM_TEXT_SIZE, MAXIMUM_TEXT_SIZE);
            newLabel.text = roundedDistance.ToString() + "m";
        }

        private void TellUIToUpdateScanButtonVisibility(bool shouldBeVisible)
        {
            EventManager.UIEvent.UpdateScanButtonVisibility.RaiseEvent(shouldBeVisible);
        }

        private void DestroyAllGeneratedMeshes()
        {
            EventManager.AppEvent.DestroyAllGeneratedMeshes.RaiseEvent();
        }
    }
}

