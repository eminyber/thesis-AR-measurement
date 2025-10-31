using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Util.Enums;

namespace ARMeasurementApp.Scripts.Util.Formatters
{
    public class AnchorPointDistanceCalculationModeEnumFormatter : IEnumToStringFormatter<AnchorPointDistanceCalculationMode>
    {
        public string ToString(AnchorPointDistanceCalculationMode enumValue)
        {
            string resolvedStr;
            switch (enumValue)
            {
                case AnchorPointDistanceCalculationMode.DirectDistance:
                    resolvedStr = "Direct Distance";
                    break;

                case AnchorPointDistanceCalculationMode.HorizontalDistance:
                    resolvedStr = "Horizontal Distance";
                    break;

                case AnchorPointDistanceCalculationMode.VerticalDistance:
                    resolvedStr = "Vertical Distance";
                    break;

                default:
                    resolvedStr = string.Empty;
                    break;
            }

            return resolvedStr;
        }
    }
}