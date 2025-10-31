namespace ARMeasurementApp.Scripts.Interfaces
{
    public interface IEnumToStringFormatter<T>
    {
        public string ToString(T enumValue);
    }
}