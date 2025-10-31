using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Properties;

using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Managers
{
    public class MeasurementLineManager : MonoBehaviour
    {
        [SerializeField] LineRenderer _lineRenderPrefab;
        [SerializeField] float _lineWidth = 0.012f;

        private List<MeasurementLineProperties> _measurementLines = new List<MeasurementLineProperties>();

        public int MeasurementLinesCount => _measurementLines.Count;
        void OnDisable()
        {
            if (_measurementLines.Count > 0) 
                DeleteAllMeasurementLines();
        }

        void Start()
        {
            if (_lineRenderPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasurementLineManager: _lineRenderPrefab is null");
                enabled = false;
                return;
            }

            if (_lineWidth <= 0.0f)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasurementLineManager: _lineWidth is set to zero or less");
                enabled = false;
                return;
            }
        }

        public MeasurementLineProperties CreateNewMeasurementLine(bool isContinous)
        {
            if (!enabled) return null;

            LineRenderer newLineRenderer = Instantiate(_lineRenderPrefab);
            if (newLineRenderer == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in MeasurementLineManager -> CreateNewMeasurementLine: A new LineRenderer could not be initialized");
                return null;
            }
            ApplyLineRenderSettings(newLineRenderer);

            MeasurementLineProperties lineProperties = new MeasurementLineProperties(newLineRenderer, isContinous);
            _measurementLines.Add(lineProperties);
           
            return lineProperties;
        }

        public MeasurementLineProperties GetMeasurementLine(int index)
        {
            if (!enabled || index < 0 || index >= _measurementLines.Count) return null;

            return _measurementLines[index];
        }

        public MeasurementLineProperties GetLastMeasurementLine()
        {
            return GetMeasurementLine(_measurementLines.Count - 1);
        }

        public void DeleteMeasurementLine(int index)
        {
            if (!enabled || index < 0 || index >= _measurementLines.Count) return;

            Destroy(_measurementLines[index].LineRenderer);
            _measurementLines.RemoveAt(index);
        }

        public void DeleteLastMeasurementLine()
        {
            DeleteMeasurementLine(_measurementLines.Count - 1);
        }

        public void DeleteAllMeasurementLines()
        {
            if (!enabled) return;

            foreach (MeasurementLineProperties measurementLine in _measurementLines)
            {
                Destroy(measurementLine.LineRenderer);
            }
            _measurementLines.Clear();
        }

        public MeasurementLineProperties ResolveMeasurementLineByCurrentMeasurementMode(bool isMeasurementModeContinuous)
        {
            if (!enabled) return null;

            if (!HasMeasurementLines()) 
                return CreateNewMeasurementLine(isMeasurementModeContinuous);

            var lastMeasurementLine = _measurementLines[_measurementLines.Count - 1];

            if (lastMeasurementLine.IsContinuous == true && isMeasurementModeContinuous == true)
            {
                return lastMeasurementLine;
            }

            if (lastMeasurementLine.IsContinuous == false && isMeasurementModeContinuous == false)
            {
                if (lastMeasurementLine.LineRenderer.positionCount > 1)
                {
                    return CreateNewMeasurementLine(isMeasurementModeContinuous);
                }
                
                return lastMeasurementLine;
            }

            if (lastMeasurementLine.LineRenderer.positionCount < 2)
            {
                lastMeasurementLine.SetIsContinous(!lastMeasurementLine.IsContinuous);
                return lastMeasurementLine;
            }

            return CreateNewMeasurementLine(isMeasurementModeContinuous);
        }

        public bool HasMeasurementLines()
        {
            if (!enabled) return false;

            return _measurementLines.Count > 0;
        }

        private void ApplyLineRenderSettings(LineRenderer lineRenderer)
        {
            if (lineRenderer == null) return;

            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
        }
    }
}


