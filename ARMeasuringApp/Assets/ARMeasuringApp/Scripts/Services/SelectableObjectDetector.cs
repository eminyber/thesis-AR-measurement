using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Services
{
    public class SelectableObjectDetector
    {
        public bool IsSelectableObjectAtPosition(Vector2 position, Camera camera)
        {
            if (camera == null) return false;

            Ray ray = camera.ScreenPointToRay(position);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                ISelectable selectable = hit.transform.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

