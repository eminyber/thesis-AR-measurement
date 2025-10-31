using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Util
{
    public static class PositionFilteringUtils
    {
        public static List<Vector3> RemovePositionsWithinLevelRange(List<Vector3> positions, float levelPosition, float levelOffset, Vector3 axisFilter)
        {
            if (positions == null) return new List<Vector3>();

            var filteredPositions = new List<Vector3>();
            foreach (Vector3 position in positions)
            {
                float pointPositionInDesiredAxis = Vector3.Dot(position, axisFilter);
                if ((levelPosition - levelOffset) <= pointPositionInDesiredAxis && pointPositionInDesiredAxis <= (levelPosition + levelOffset))
                {
                    continue;
                }

                filteredPositions.Add(position);
            }

            return filteredPositions;
        }

        public static List<Vector3> RemovePositionsOutsideLevelRange(List<Vector3> positions, float levelPosition, float levelOffset, Vector3 axisFilter)
        {
            if (positions == null) return new List<Vector3>();

            var filteredPositions = new List<Vector3>();
            foreach (Vector3 position in positions)
            {
                float pointPositionInDesiredAxis = Vector3.Dot(position, axisFilter);
                if ((levelPosition - levelOffset) <= pointPositionInDesiredAxis && pointPositionInDesiredAxis <= (levelPosition + levelOffset))
                {
                    filteredPositions.Add(position);
                }
            }

            return filteredPositions;

        }

        public static List<Vector3> RemovePositionsUnderLevel(List<Vector3> positions, float levelPosition, float levelOffset, Vector3 axisFilter)
        {
            if (positions == null) return new List<Vector3>();

            var filteredPositions = new List<Vector3>();
            foreach (Vector3 position in positions)
            {
                float pointPositionInDesiredAxis = Vector3.Dot(position, axisFilter);
                if (pointPositionInDesiredAxis > (levelPosition + levelOffset))
                {
                    filteredPositions.Add(position);
                }
            }

            return filteredPositions;
        }

        public static List<Vector3> RemovePositionsNotOnPlane(List<Vector3> positions, Vector3 planeCenterPosition, Vector3 planeNormal, float planeLevelOffset)
        {
            if (positions == null) return new List<Vector3>();

            var filteredPositions = new List<Vector3>();
            foreach (Vector3 position in positions)
            {
                float perpendicularDistanceToPlane = Vector3.Dot((position - planeCenterPosition), planeNormal);
                if (Mathf.Abs(perpendicularDistanceToPlane) <= planeLevelOffset)
                {
                    filteredPositions.Add(position);
                }
            }

            return filteredPositions;
        }

        public static List<Vector3> RemovePositionsOutsideMarkedArea(List<Vector3> positions, List<Vector2> markedAreaVertices)
        {
            if (positions == null || markedAreaVertices == null) return new List<Vector3>();

            var filteredPositions = new List<Vector3>();
            foreach (Vector3 position in positions)
            {
                var xzPointCloudPosition = new Vector2(position.x, position.z);
                bool isInPolygon = MathUtils.IsPointInPolygon(markedAreaVertices, xzPointCloudPosition);
                if (isInPolygon)
                {
                    filteredPositions.Add(position);
                }
            }

            return filteredPositions;
        }
    }
}

