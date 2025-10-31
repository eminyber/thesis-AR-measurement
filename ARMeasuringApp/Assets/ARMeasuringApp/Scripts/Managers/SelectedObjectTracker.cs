namespace ARMeasurementApp.Scripts.Managers
{
    public class SelectedObjectTracker<T>
    {
        public T CurrentSelectedObject { get; private set; } = default(T);

        public void SetCurrentSelectedObject(T obj)
        {
            CurrentSelectedObject = obj;
        }

        public void ClearCurrentSelectedObject()
        {
            CurrentSelectedObject = default(T);
        }
    }
}