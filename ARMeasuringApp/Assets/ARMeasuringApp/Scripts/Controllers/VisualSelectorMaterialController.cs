using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Controllers
{
    [RequireComponent(typeof(MeshRenderer))]
    public class VisualSelectorMaterialController : MonoBehaviour, ISelectableVisualizer
    {
        [SerializeField] private Material _selectedMaterial;
        [SerializeField] private Material _defaultMaterial;

        private MeshRenderer _meshRenderer; 

        void OnEnable()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            if (_selectedMaterial == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in VisualSelectorMaterialController: _selectedMaterial is null");
                enabled = false;
                return;
            }

            if (_defaultMaterial == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in VisualSelectorMaterialController: _defaultMaterial is null");
                enabled = false;
                return;
            }
        }

        public void ApplySelectedVisual()
        {
            if (!enabled) return;

            _meshRenderer.material = _selectedMaterial;
        }

        public void RemoveSelectedVisual()
        {
            if (!enabled) return;

            _meshRenderer.material = _defaultMaterial;
        }
    }
}

