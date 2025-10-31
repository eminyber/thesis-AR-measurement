using System.Collections.Generic;
using UnityEngine;

namespace ARMeasurementApp.Scripts.Util
{
    public static class MathUtils
    {
        public static bool IsPointInPolygon(List<Vector2> polygonVertices, Vector2 point)
        {
            int n = polygonVertices.Count;
            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                Vector2 vi = polygonVertices[i];
                Vector2 vj = polygonVertices[j];

                // Check if the point is inside the polygon
                if ((vi.y > point.y) != (vj.y > point.y) &&
                    (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}

