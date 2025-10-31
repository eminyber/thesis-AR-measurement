using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Managers;

using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Controllers
{
    public class BoxRenderController : MonoBehaviour
    {
        [SerializeField] MeasurementLineManager _measurementLineManager;

        private LineRenderController _lineRenderController = new LineRenderController();

        void Start()
        {
            if (_measurementLineManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in BoxRenderController: _measurementLineManager is null");
                enabled = false;
                return;
            }

            for (int i = 0; i < 12; i++)
            {
                _measurementLineManager.CreateNewMeasurementLine(false);
            }
        }

        //
        // Send in the corner positions in the following order.
        //
        //    [3,7] +--------+ [2,6]  (Low z, High z)
        //          |        |
        //          |        |
        //          |        |
        //    [0,4] +--------+ [1,5]
        //
        public void RenderBox(List<Vector3> cornerPositions)
        {
            if (!enabled || cornerPositions.Count != 8) return;

            int measurementLineIndex = 0;
            // 0-3
            for (int i = 0; i < 4; i++)
            {
                _lineRenderController.DrawALine(_measurementLineManager.GetMeasurementLine(measurementLineIndex).LineRenderer, cornerPositions[i], cornerPositions[(i + 1) % 4]);
                measurementLineIndex++;
            }

            //4-7
            for (int i = 4; i < 8; i++)
            {
                _lineRenderController.DrawALine(_measurementLineManager.GetMeasurementLine(measurementLineIndex).LineRenderer, cornerPositions[i], cornerPositions[((i + 1) % 4) + 4]);
                measurementLineIndex++;
            }

            // Connects the front and top sides: I.e [0,4],,,
            for (int i = 0; i < 4; i++)
            {
                _lineRenderController.DrawALine(_measurementLineManager.GetMeasurementLine(measurementLineIndex).LineRenderer, cornerPositions[i], cornerPositions[(i + 4)]);
                measurementLineIndex++;
            }
        }

        public void EraseRenderedBox()
        {
            if (!enabled) return;

            for (int i = 0; i < _measurementLineManager.MeasurementLinesCount; i++)
            {
                _lineRenderController.ClearLines(_measurementLineManager.GetMeasurementLine(i).LineRenderer);
            }
        }
    }
}

