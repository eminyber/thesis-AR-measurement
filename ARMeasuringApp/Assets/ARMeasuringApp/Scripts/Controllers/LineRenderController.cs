using UnityEngine;

namespace ARMeasurementApp.Scripts.Controllers
{
    public class LineRenderController
    {
        public int GetPoisitonCount(LineRenderer lineRenderer)
        {
            if (lineRenderer == null) return -1;

            return lineRenderer.positionCount;
        }

        public void DrawALine(LineRenderer lineRenderer, Vector3 starPosition, Vector3 endPosition)
        {
            if (lineRenderer == null) return;

            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, starPosition);
            lineRenderer.SetPosition(1, endPosition);
        }

        public void AddALine(LineRenderer lineRenderer, Vector3 starPosition, Vector3 endPosition)
        {
            if (lineRenderer == null) return;

            lineRenderer.positionCount += 2;

            lineRenderer.SetPosition(lineRenderer.positionCount - 2, starPosition);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPosition);
        }

        public void AddPointToLine(LineRenderer lineRenderer, Vector3 position)
        {
            if (lineRenderer == null) return;

            lineRenderer.positionCount++;

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
        }

        public void UpdatePointInLine(LineRenderer lineRenderer, int pointIndex,  Vector3 newPosition)
        {
            if (lineRenderer == null) return;

            if (lineRenderer.positionCount == 0 || pointIndex > lineRenderer.positionCount - 1) return;

            lineRenderer.SetPosition(pointIndex, newPosition);
        }

        public void DeleteLastPoint(LineRenderer lineRenderer)
        {
            if (lineRenderer == null || lineRenderer.positionCount < 1) return;
            
            lineRenderer.positionCount--;
        }

        public void ClearLines(LineRenderer lineRenderer)
        {
            if (lineRenderer == null) return;
            
            lineRenderer.positionCount = 0;
        }
    }
}

