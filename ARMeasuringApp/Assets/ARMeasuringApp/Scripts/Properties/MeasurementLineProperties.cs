using UnityEngine;

namespace ARMeasurementApp.Scripts.Properties
{
    public class MeasurementLineProperties
    {
        private LineRenderer _lineRenderer;
        private bool _isContinuous;

        public LineRenderer LineRenderer => _lineRenderer;
        public bool IsContinuous => _isContinuous;

        public MeasurementLineProperties(LineRenderer lineRenderer, bool isContinuous)
        {
            _lineRenderer = lineRenderer;
            _isContinuous = isContinuous;
        }

        public void SetIsContinous(bool isContinuous)
        {
            _isContinuous = isContinuous;
        }
    }
}