using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Services
{
    public class ARPlaneInfoProvider
    {
        public string GetPlaneWidhtAsString(ARPlane plane)
        {
            if (plane == null) return string.Empty;

            return plane.size.x.ToString();
        }

        public string GetPlaneHeightAsString(ARPlane plane)
        {
            if (plane == null) return string.Empty;

            return plane.size.y.ToString();
        }

        public string GetPlaneDimensionInfoAsString(ARPlane plane)
        {
            if (plane == null) return string.Empty;

            return $"Plane Width: {GetPlaneWidhtAsString(plane)}m, Plane Height: {GetPlaneHeightAsString(plane)}m";
        }

        public string GetPlaneDimensionInfoAsStringWithLineColors(ARPlane plane, string widthLineColor, string heightLineColors)
        {
            if (plane == null) return string.Empty;

            return $"Plane Width ({widthLineColor} line): {GetPlaneWidhtAsString(plane)}m, Plane Height ({heightLineColors} line): {GetPlaneHeightAsString(plane)}m";
        }

        public Vector3 GetPlaneWidthStartPosition(ARPlane plane)
        {
            if (plane == null) return default;

            return plane.center - getPlaneWidthOffset(plane);
        }

        public Vector3 GetPlaneWidthEndPosition(ARPlane plane)
        {
            if (plane == null) return default;

            return plane.center + getPlaneWidthOffset(plane);
        }

        public Vector3 GetPlaneHeightStartPosition(ARPlane plane)
        {
            if (plane == null) return default;

            return plane.center - getPlaneHeightOffset(plane);
        }

        public Vector3 GetPlaneHeightEndPosition(ARPlane plane)
        {
            if (plane == null) return default;

            return plane.center + getPlaneHeightOffset(plane);
        }

        private Vector3 getPlaneWidthOffset(ARPlane plane)
        {
            return (plane.transform.right * plane.extents.x);
        }
        private Vector3 getPlaneHeightOffset(ARPlane plane)
        {
            return (plane.transform.forward * plane.extents.y);
        }
    }
}

