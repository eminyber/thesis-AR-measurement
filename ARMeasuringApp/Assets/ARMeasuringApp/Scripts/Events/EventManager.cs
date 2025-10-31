using ARMeasurementApp.Events.UIEvents;
using ARMeasurementApp.Scripts.Events.AppEvents;
using ARMeasurementApp.Scripts.Events.InputEvents;

namespace ARMeasurementApp.Scripts.Events
{
    public static class EventManager
    {
        public static readonly AppEvent AppEvent = new();

        public static readonly ButtonEvent ButtonClickEvent = new();
        public static readonly TouchEvent TouchEvent = new();

        public static readonly UIEvent UIEvent = new();
    }
}

