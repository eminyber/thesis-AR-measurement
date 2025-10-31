
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Util;

using System;

using UnityEngine;
using TMPro;

namespace ARMeasurementApp.Scripts.Systems
{
    
    public class PointPlacementMeasurementSystem : MonoBehaviour
    {
        [SerializeField] GameObjectsManager _measurementPointManager;
        [SerializeField] TextMeshProsManager _labelManager;
        [SerializeField] MeasurementLineManager _measurementLineManager;

        private LineRenderController _lineRenderController = new LineRenderController();
        private bool _isMeasurementsContinuous = true;

        private const float LABEL_UPSCALAR = 0.05f;
        private const int FLOAT_ROUND_PRECISION = 4;

        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.AddListener(PlaceMeasurementPoint);

            EventManager.ButtonClickEvent.DeleteLatestMeasurementPoint.AddListener(RemoveLastMeasurementPoint);
            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
            EventManager.ButtonClickEvent.ToggleContinuousMeasurementMode.AddListener(ToggleIsMeasurementContinuous);
        }

        void OnDisable()
        {
            EventManager.TouchEvent.UserTappedToSendARPlaneIntersectionPose.RemoveListener(PlaceMeasurementPoint);

            EventManager.ButtonClickEvent.DeleteLatestMeasurementPoint.RemoveListener(RemoveLastMeasurementPoint);
            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);
            EventManager.ButtonClickEvent.ToggleContinuousMeasurementMode.RemoveListener(ToggleIsMeasurementContinuous);
        }

        void Start()
        {
            if (_measurementPointManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PointPlacementMeasurementSystem: _measurementPointManager is null");
                enabled = false;
                return;
            }

            if (_labelManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PointPlacementMeasurementSystem: _labelManager is null");
                enabled = false;
                return;
            }

            if (_measurementLineManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PointPlacementMeasurementSystem: _measurementLineManager is null");
                enabled = false;
                return;
            }

            TellUIContinuousMeasurementStateChanged();
        }

        private void PlaceMeasurementPoint(Pose userTouchPose)
        {
            if (userTouchPose == null) return;

            var newMeasurementPoint = _measurementPointManager.CreateNewObject(userTouchPose.position, userTouchPose.rotation);
            if (newMeasurementPoint == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PointPlacementMeasurementSystem -> placeMeasurementPoint: A new measurement point wasn't initialized");
                return;
            }

            var currentMeasurmentLinePositionCount = UpdateMeasurementLines(userTouchPose.position);
            ConductMeasurement(currentMeasurmentLinePositionCount);
        }

        private void RemoveLastMeasurementPoint()
        {
            if (!_measurementLineManager.HasMeasurementLines()) return;

            _measurementPointManager.DeleteLastObject();

            var LatesMeasurementLine = _measurementLineManager.GetLastMeasurementLine();
            if (_lineRenderController.GetPoisitonCount(LatesMeasurementLine.LineRenderer) == 1)
            {
                _measurementLineManager.DeleteLastMeasurementLine();
                return;
            }

            _lineRenderController.DeleteLastPoint(LatesMeasurementLine.LineRenderer);
            _labelManager.DeleteLastTextMesh();
        }

        private void ResetSystem()
        {
            if (!_measurementLineManager.HasMeasurementLines()) return;

            _measurementPointManager.DeleteAllObjects();
            _labelManager.DeleteAllTextMeshes();
            _measurementLineManager.DeleteAllMeasurementLines();
        }

        private void ToggleIsMeasurementContinuous()
        {
            _isMeasurementsContinuous = !_isMeasurementsContinuous;

            TellUIContinuousMeasurementStateChanged();
        }

        private int UpdateMeasurementLines(Vector3 newMeasurementPointPosition)
        {
            var measurementLine = _measurementLineManager.ResolveMeasurementLineByCurrentMeasurementMode(_isMeasurementsContinuous);
            if (measurementLine == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PointPlacementMeasurementSystem -> UpdateMeasurementLines: Could not resolve a measurementline");
                return 0;
            }

            _lineRenderController.AddPointToLine(measurementLine.LineRenderer, newMeasurementPointPosition);
            return _lineRenderController.GetPoisitonCount(measurementLine.LineRenderer);
        }

        private void ConductMeasurement(int numberOfMeasurementLinePositions)
        {
            if (numberOfMeasurementLinePositions < 2) return;

            Transform sourceMeasurementPoint = _measurementPointManager.GetObject(_measurementPointManager.ObjectCount - 2).transform;
            Transform destinationMeasurementPoint = _measurementPointManager.GetObject(_measurementPointManager.ObjectCount - 1).transform;

            var distance = (float)Math.Round(Vector3.Distance(destinationMeasurementPoint.position, sourceMeasurementPoint.position), FLOAT_ROUND_PRECISION);

            CreateNewMeasurementLabel(distance, sourceMeasurementPoint, destinationMeasurementPoint);
        }

        private void CreateNewMeasurementLabel(float distance, Transform sourceMeasurementPoint, Transform destinationMeasurementPoint)
        {
            var labelPosition = TextAlignmentUtils.CalculateLabelPositionAboveMidpointOnPlane(sourceMeasurementPoint.position, destinationMeasurementPoint.position, sourceMeasurementPoint.up, LABEL_UPSCALAR);
            var labelRotation = TextAlignmentUtils.CalculateLabelRotationOnPlane(sourceMeasurementPoint.position, destinationMeasurementPoint.position, sourceMeasurementPoint.up);

            TextMeshPro newLabel = _labelManager.CreateNewTextMesh(labelPosition, labelRotation);
            if (newLabel == null) 
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in PointPlacementMeasurementSystem -> CreateMeasurementLabel: A new TextMesh could not be created");
                return;
            }

            newLabel.text = distance.ToString() + "m";
        }

        private void TellUIContinuousMeasurementStateChanged()
        {
            EventManager.UIEvent.ContinuousMeasurementModeStateChanged.RaiseEvent(_isMeasurementsContinuous);
        }
    }
}
