using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Events.Eventargs
{
    public class PlaneAndPoseOnPlaneEventArgs : EventArgs
    {
        public ARPlane Plane { get; }
        public Pose Pose { get; }

        public PlaneAndPoseOnPlaneEventArgs(ARPlane plane, Pose pose)
        {
            Plane = plane;
            Pose = pose;
        }
    }
}
