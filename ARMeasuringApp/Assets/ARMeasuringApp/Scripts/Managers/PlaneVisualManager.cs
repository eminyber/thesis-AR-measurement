using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Managers
{
    public class PlaneVisualManager
    {
        public void ApplySelectedStateVisual(ARPlane plane)
        {
            if (plane == null) return;

            ISelectableVisualizer selectableVisualizer = plane.GetComponent<ISelectableVisualizer>();
            if (selectableVisualizer != null)
                selectableVisualizer.ApplySelectedVisual();

        }

        public void RemoveSelectedStateVisual(ARPlane plane)
        {
            if (plane == null) return;

            ISelectableVisualizer selectableVisualizer = plane.GetComponent<ISelectableVisualizer>();
            if (selectableVisualizer != null)
                selectableVisualizer.RemoveSelectedVisual();
        }
    }
}

