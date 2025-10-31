using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Managers;
using ARMeasurementApp.Scripts.Properties;

using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Objects
{
    public class OMBB
    {
        private MeasurementLineManager _measurementLineManager;
        private LineRenderController _lineRenderController;

        private List<Vector3> _boxCornerPositions = new List<Vector3>();

        private Vector3 _center = default;
        private Vector3 _axisX = default;
        private Vector3 _axisY = default;
        private Vector3 _axisZ = default;
        private Vector3 _extents = default;

        public OMBB(MeasurementLineManager measurementLineManager, LineRenderController lineRenderController)
        {
            if (measurementLineManager == null || lineRenderController == null) return;

            _measurementLineManager = measurementLineManager;
            _lineRenderController = lineRenderController;

            for (int i = 0; i < 12; i++) 
            {
                _measurementLineManager.CreateNewMeasurementLine(false);
            }
        }

        public void UpdateProperties(Vector3 center, Vector3 axisX, Vector3 axisY, Vector3 axisZ, Vector3 extents)
        {
            _center = center;
            _axisX = axisX;
            _axisY = axisY;
            _axisZ = axisZ;
            _extents = extents;

            ComputeCornerVertices();
        }

        public List<(float Distance, Vector3 SourcePosition, Vector3 DestinationPosition)> GetDimensions()
        {
            if (_boxCornerPositions.Count == 0) return new List<(float Distance, Vector3 SourcePosition, Vector3 DestinationPosition)> ();

            List<(float Distance, Vector3 SourcePosition, Vector3 DestinationPosition)> dimensionInfo = new List<(float Distance, Vector3 SourcePosition, Vector3 DestinationPosition)>();
            (float distance, Vector3 sourcePosition, Vector3 destinationPosition) distanceInfo;

            // Todo -> clean this up, should be doable with for loop
            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[0], _boxCornerPositions[1]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[1], _boxCornerPositions[2]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[2], _boxCornerPositions[3]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[3], _boxCornerPositions[0]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[4], _boxCornerPositions[5]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[5], _boxCornerPositions[6]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[6], _boxCornerPositions[7]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[7], _boxCornerPositions[4]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[0], _boxCornerPositions[4]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[1], _boxCornerPositions[5]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[2], _boxCornerPositions[6]);
            dimensionInfo.Add(distanceInfo);

            distanceInfo = ComputeDistanceInfo(_boxCornerPositions[3], _boxCornerPositions[7]);
            dimensionInfo.Add(distanceInfo);

            return dimensionInfo;
        }

        public void Draw()
        {
            if (_boxCornerPositions.Count == 0) return;

            int measurementLine = 0;
            MeasurementLineProperties measurementLineProperties;

            // Bottom of box
            for (int i = 0; i < 4; i++)
            {
                measurementLineProperties = _measurementLineManager.GetMeasurementLine(measurementLine);
                _lineRenderController.DrawALine(measurementLineProperties.LineRenderer, _boxCornerPositions[i], _boxCornerPositions[(i + 1) % 4]);
                measurementLine++;
            }

            // Top of Box
            for (int i = 4; i < 8; i++)
            {
                measurementLineProperties = _measurementLineManager.GetMeasurementLine(measurementLine);
                _lineRenderController.DrawALine(measurementLineProperties.LineRenderer, _boxCornerPositions[i], _boxCornerPositions[(i + 1) % 4 + 4]);
                measurementLine++;
            }

            // Connect Top and Bottom
            for (int i = 0; i < 4; i++)
            {
                measurementLineProperties = _measurementLineManager.GetMeasurementLine(measurementLine);
                _lineRenderController.DrawALine(measurementLineProperties.LineRenderer, _boxCornerPositions[i], _boxCornerPositions[i + 4]);
                measurementLine++;
            }
        }

        public void Erase()
        {
            for (int i = 0; i < _measurementLineManager.MeasurementLinesCount; i++)
            {
                _lineRenderController.ClearLines(_measurementLineManager.GetMeasurementLine(i).LineRenderer);
            }
        }

        private void ComputeCornerVertices()
        {
            var A = _center - _extents.z * _axisZ - _extents.x * _axisX - _axisY * _extents.y;
            var B = _center - _extents.z * _axisZ + _extents.x * _axisX - _axisY * _extents.y;
            var C = _center - _extents.z * _axisZ + _extents.x * _axisX + _axisY * _extents.y;
            var D = _center - _extents.z * _axisZ - _extents.x * _axisX + _axisY * _extents.y;

            var E = _center + _extents.z * _axisZ - _extents.x * _axisX - _axisY * _extents.y;
            var F = _center + _extents.z * _axisZ + _extents.x * _axisX - _axisY * _extents.y;
            var G = _center + _extents.z * _axisZ + _extents.x * _axisX + _axisY * _extents.y;
            var H = _center + _extents.z * _axisZ - _extents.x * _axisX + _axisY * _extents.y;

            _boxCornerPositions.Clear();

            _boxCornerPositions.Add(A); // 0
            _boxCornerPositions.Add(B); // 1
            _boxCornerPositions.Add(C); // 2
            _boxCornerPositions.Add(D); // 3
            _boxCornerPositions.Add(E); // 4
            _boxCornerPositions.Add(F); // 5
            _boxCornerPositions.Add(G); // 6
            _boxCornerPositions.Add(H); // 7
        }

        private (float Distance, Vector3 SourcePosition, Vector3 DestinationPosition) ComputeDistanceInfo(Vector3 sourcePosition, Vector3 destinationPosition)
        {
            float distance = Vector3.Distance(destinationPosition, sourcePosition);
            return (distance, sourcePosition, destinationPosition);
        }
    }
}

