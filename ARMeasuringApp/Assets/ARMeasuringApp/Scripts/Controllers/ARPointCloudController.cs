using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Controllers
{
    public class ARPointCloudController
    {
        public List<Vector3> GetFeaturePointsPositions(List<ARPointCloud> pointClouds)
        {
            if (pointClouds == null) return new List<Vector3>();

            var featurePointPositions = new List<Vector3>();
            foreach (ARPointCloud pointCloud in pointClouds)
            {
                if (pointCloud.positions.HasValue)
                    featurePointPositions.AddRange(pointCloud.positions.Value.ToArray());
            }

            return featurePointPositions;
        }

        public List<Vector3> GetFeaturePointsPositionsAtOrAboveConfidenceValue(List<ARPointCloud> pointClouds, float minimumConfidenceValue)
        {
            if (pointClouds == null || minimumConfidenceValue < 0 || minimumConfidenceValue > 1) return new List<Vector3>();

            var featurePointPositions = new List<Vector3>();
            foreach (ARPointCloud pointCloud in pointClouds)
            {
                if (!pointCloud.confidenceValues.HasValue) continue;

                for (int i = 0; i < pointCloud.confidenceValues.Value.Length; i++)
                {
                    if (pointCloud.confidenceValues.Value[i] >= minimumConfidenceValue)
                        featurePointPositions.Add(pointCloud.positions.Value[i]);
                }
            }

            return featurePointPositions;
        }
    }
}

