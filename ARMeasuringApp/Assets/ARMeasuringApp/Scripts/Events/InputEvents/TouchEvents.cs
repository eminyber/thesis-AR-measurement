
using ARMeasurementApp.Scripts.Events.Eventargs;
using UnityEngine;

namespace ARMeasurementApp.Scripts.Events.InputEvents
{
    public class TouchEvent
    {
        public readonly EventWithHandler<PlaneAndPoseOnPlaneEventArgs> UserTappedWithinARPlane = new();
        public readonly Event<Pose> UserTappedToSendCameraPose = new();
        public readonly Event<Pose> UserTappedToSendARPlaneIntersectionPose = new();

        // Currently not in use
        public readonly Event OnUserInteractedWithSelectableObject = new();
    }
}

