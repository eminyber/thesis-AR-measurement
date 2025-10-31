using UnityEngine;

namespace ARMeasurementApp.Scripts.Util
{
    public static class TextAlignmentUtils
    {
        public static Vector3 CalculateLabelPositionAboveMidpointInWorld(Vector3 sourcePosition, Vector3 destinationPosition, float upScalar)
        {
            if (sourcePosition == null || destinationPosition == null) return Vector3.zero;

            var directionVector = (destinationPosition - sourcePosition).normalized;
            var normalVector = Vector3.Cross(directionVector, Vector3.up).normalized;

            var upVector = Vector3.Cross(normalVector, directionVector).normalized;

            var labelPosition = ((sourcePosition + destinationPosition) / 2) + upVector * upScalar;
            
            return labelPosition;
        }

        public static Vector3 CalculateLabelPositionAboveMidpointOnPlane(Vector3 sourcePosition, Vector3 destinationPosition, Vector3 planeNormal, float upScalar)
        {
            if (sourcePosition == null || destinationPosition == null ||  planeNormal == null) return Vector3.zero;

            var directionVector = (destinationPosition - sourcePosition).normalized;
            var upVector = Vector3.Cross(directionVector, planeNormal.normalized).normalized;

            var labelPosition = ((sourcePosition + destinationPosition) / 2) + upVector * upScalar;

            return labelPosition;
        }

        public static Quaternion CalculateLabelRotationInWorld(Vector3 sourcePosition, Vector3 destinationPosition)
        {
            if (sourcePosition == null || destinationPosition == null) return Quaternion.identity;

            var directionVector = (destinationPosition - sourcePosition).normalized;
            var normalVector = Vector3.Cross(directionVector, Vector3.up).normalized;

            var upVector = Vector3.Cross(normalVector, directionVector).normalized;

            var labelRotation = Quaternion.LookRotation(normalVector, upVector);

            return labelRotation;
        }

        public static Quaternion CalculateLabelRotationOnPlane(Vector3 sourcePosition, Vector3 destinationPosition, Vector3 planeNormal)
        {
            if (sourcePosition == null || destinationPosition == null || planeNormal == null) return Quaternion.identity;

            var directionVector = (destinationPosition - sourcePosition).normalized;
            var upVector = Vector3.Cross(directionVector, planeNormal.normalized).normalized;

            var labelRotation = Quaternion.LookRotation(-planeNormal.normalized, upVector);

            return labelRotation;
        }
    }
}

