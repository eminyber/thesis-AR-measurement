using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Util;

using System.Collections.Generic;

using System;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;



namespace ARMeasurementApp.Scripts.Systems
{
    public class CameraTrackingMeasurementSystem : MonoBehaviour
    {
        [SerializeField] ARAnchorManager _arAnchorManager;
        [SerializeField] MeasurementLineManager _measurementLineManager;
        [SerializeField] TextMeshProsManager _labelManager;

        private List<Pose> _phoneWorldPoses = new List<Pose>();
        private List<ARAnchor> _spawnedAnchors = new List<ARAnchor>();
        private Dictionary<ARAnchor, (int measurementLineIndex, int positionInMeasurementLine)> _anchorMeasurementLineInfo = new Dictionary<ARAnchor, (int measurementLineIndex, int positionInline)>();

        private LineRenderController _lineRenderController = new LineRenderController();
        private bool _isMeasurementsContinuous = false;

        private const float LABEL_UPSCALAR = 0.05f;
        private const int FLOAT_ROUND_PRECISION = 4;

        void OnEnable()
        {
            EventManager.TouchEvent.UserTappedToSendCameraPose.AddListener(AddNewMeasurementPoint);

            EventManager.AppEvent.OnARAnchorAdded.AddListener(HandleAnchorAdded);
            EventManager.AppEvent.OnARAnchorUpdated.AddListener(HandleAnchorUpdated);
            EventManager.AppEvent.OnARAnchorRemoved.AddListener(HandleAnchorRemoved);

            EventManager.ButtonClickEvent.DeleteLatestMeasurementPoint.AddListener(RemoveLastMeasurementPoint);
            EventManager.ButtonClickEvent.ResetCurrentScene.AddListener(ResetSystem);
            EventManager.ButtonClickEvent.ToggleContinuousMeasurementMode.AddListener(ToggleIsMeasurementContinuous);
        }

        void OnDisable()
        {
            EventManager.TouchEvent.UserTappedToSendCameraPose.RemoveListener(AddNewMeasurementPoint);

            EventManager.AppEvent.OnARAnchorAdded.RemoveListener(HandleAnchorAdded);
            EventManager.AppEvent.OnARAnchorUpdated.RemoveListener(HandleAnchorUpdated);
            EventManager.AppEvent.OnARAnchorRemoved.RemoveListener(HandleAnchorRemoved);

            EventManager.ButtonClickEvent.DeleteLatestMeasurementPoint.RemoveListener(RemoveLastMeasurementPoint);
            EventManager.ButtonClickEvent.ResetCurrentScene.RemoveListener(ResetSystem);
            EventManager.ButtonClickEvent.ToggleContinuousMeasurementMode.RemoveListener(ToggleIsMeasurementContinuous);
        }

        void Start()
        {
            if (_arAnchorManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasureWithCameraPoseSystem: _anchorManager is null.");
                enabled = false;
                return;
            }

            if (_measurementLineManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasureWithCameraPoseSystem: _measurementLineManager is null.");
                enabled = false;
                return;
            }

            if (_labelManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasureWithCameraPoseSystem: _labelManager is null.");
                enabled = false;
                return;
            }

            TellUIContinuousMeasurementStateChanged();
        }

        private async void AddNewMeasurementPoint(Pose pose)
        {
            if (pose == null) return;

            // Spawn an anchor to represent the new measurement point
            var result = await _arAnchorManager.TryAddAnchorAsync(pose);
            if (!result.status.IsSuccess())
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasureWithCameraPoseSystem -> AddNewMeasurementPoint: A new anchor could not be initialized");
                return;
            }

            _phoneWorldPoses.Add(pose);
        }

        private void HandleAnchorAdded(ARAnchor anchor)
        {
            if (anchor == null) return;

            _spawnedAnchors.Add(anchor);

            var currentMeasurmentLinePositionCount = UpdateMeasurementLines(_phoneWorldPoses[_phoneWorldPoses.Count - 1].position);
            _anchorMeasurementLineInfo.Add(anchor, (_measurementLineManager.MeasurementLinesCount - 1, currentMeasurmentLinePositionCount - 1));

            ConductMeasurement(currentMeasurmentLinePositionCount);
        }

        private void HandleAnchorUpdated(ARAnchor anchor)
        {
            if (anchor == null) return;

            if (anchor.trackingState == TrackingState.Limited)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent($"Warning in MeasureWithCameraPoseSystem -> HandleAnchorUpdated: ARAnchor: {_spawnedAnchors.IndexOf(anchor)} was not updated because it's tracking was Limited.");
                return;
            }

            var measurementLineInfo = _anchorMeasurementLineInfo[anchor];
            var measurementLine = _measurementLineManager.GetMeasurementLine(measurementLineInfo.measurementLineIndex);

            _lineRenderController.UpdatePointInLine(measurementLine.LineRenderer, measurementLineInfo.positionInMeasurementLine, anchor.transform.position);
        }

        private void HandleAnchorRemoved(ARAnchor anchor)
        {
            if (anchor == null) return;

            _anchorMeasurementLineInfo.Remove(anchor);
            Destroy(anchor);
        }

        private void RemoveLastMeasurementPoint()
        {
            if (!_measurementLineManager.HasMeasurementLines()) return;

            var success = RemovedAnchorFromAnchormanager(_spawnedAnchors[_spawnedAnchors.Count - 1]);
            if (!success)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasureWithCameraPoseSystem -> RemoveLastMeasurementPoint: Acnhor: " + (_spawnedAnchors.Count - 1).ToString() + " could not be deleted.");
                return;
            }
            _spawnedAnchors.RemoveAt(_spawnedAnchors.Count - 1);
            _phoneWorldPoses.RemoveAt(_phoneWorldPoses.Count - 1);

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

            for (int i = 0; i < _spawnedAnchors.Count; i++) 
            {
                var success = RemovedAnchorFromAnchormanager(_spawnedAnchors[i]);
                if (!success)
                {
                    EventManager.AppEvent.LogError.RaiseEvent("Error in MeasureWithCameraPoseSystem -> ResetSystem: Acnhor: " + i.ToString() + " could not be deleted.");
                    continue;
                }
            }
            _spawnedAnchors.Clear();
            _phoneWorldPoses.Clear();

            _measurementLineManager.DeleteAllMeasurementLines();
            _labelManager.DeleteAllTextMeshes();
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
                EventManager.AppEvent.LogError.RaiseEvent("Error in CameraTrackingMeasurementSystem -> UpdateMeasurementLines: Could not resolve a measurementline");
                return 0;
            }

            _lineRenderController.AddPointToLine(measurementLine.LineRenderer, newMeasurementPointPosition);
            return _lineRenderController.GetPoisitonCount(measurementLine.LineRenderer);
        }

        private void ConductMeasurement(int numberOfMeasurementLinePositions)
        {
            if (numberOfMeasurementLinePositions < 2) return;

            var sourceWorldPose = _phoneWorldPoses[_phoneWorldPoses.Count - 2];
            var destinationWorldPose = _phoneWorldPoses[_phoneWorldPoses.Count - 1];

            var distance = (float)Math.Round(Vector3.Distance(destinationWorldPose.position, sourceWorldPose.position), FLOAT_ROUND_PRECISION);

            CreateMeasurementLabel(distance, sourceWorldPose, destinationWorldPose);
        }

        private void CreateMeasurementLabel(float distance, Pose sourceWorldPose, Pose destinationWorldPose)
        {
            var labelPosition = TextAlignmentUtils.CalculateLabelPositionAboveMidpointInWorld(sourceWorldPose.position, destinationWorldPose.position, LABEL_UPSCALAR);
            var labelRotation = TextAlignmentUtils.CalculateLabelRotationInWorld(sourceWorldPose.position, destinationWorldPose.position);

            TextMeshPro newLabel = _labelManager.CreateNewTextMesh(labelPosition, labelRotation);
            if (newLabel == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in CameraTrackingMeasurementSystem -> CreateMeasurementLabel: A new TextMesh could not be created");
                return;
            }

            newLabel.text = distance.ToString() + "m";
        }

        private bool RemovedAnchorFromAnchormanager(ARAnchor anchor)
        {
            return _arAnchorManager.TryRemoveAnchor(anchor);
        }

        private void TellUIContinuousMeasurementStateChanged()
        {
            EventManager.UIEvent.ContinuousMeasurementModeStateChanged.RaiseEvent(_isMeasurementsContinuous);
        }
    }
}

