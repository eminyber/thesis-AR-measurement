using ARMeasurementApp.Scripts.Events;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Managers
{
    public class MeshManager : MonoBehaviour
    {
        [SerializeField] ARMeshManager _arMeshManager;

        void OnEnable()
        {
            EventManager.ButtonClickEvent.OnSliderValueChanged.AddListener(UpdateMeshDensity);

            
            EventManager.AppEvent.EnableMeshGeneration.AddListener(OnEnableMeshGeneration);
            EventManager.AppEvent.DisableMeshGeneration.AddListener(OnDisableMeshGeneration);

            EventManager.AppEvent.RequestGeneratedMeshes.AddListener(SendGeneratedMeshes);
            EventManager.AppEvent.DestroyAllGeneratedMeshes.AddListener(OnDestroyAllGeneratedMeshes);

            if (_arMeshManager != null)
                _arMeshManager.enabled = false;
        }

        void OnDisable()
        {
            EventManager.ButtonClickEvent.OnSliderValueChanged.RemoveListener(UpdateMeshDensity);

            EventManager.AppEvent.EnableMeshGeneration.RemoveListener(OnEnableMeshGeneration);
            EventManager.AppEvent.DisableMeshGeneration.RemoveListener(OnDisableMeshGeneration);


            EventManager.AppEvent.RequestGeneratedMeshes.RemoveListener(SendGeneratedMeshes);
            EventManager.AppEvent.DestroyAllGeneratedMeshes.RemoveListener(OnDestroyAllGeneratedMeshes);

            if (_arMeshManager != null && _arMeshManager.enabled)
                _arMeshManager.enabled = false;
        }

        void Start()
        {
            if (_arMeshManager != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in MeshManager: _arMeshManager is null");
            enabled = false;
        }

        private void UpdateMeshDensity(float newDensity)
        {
            if (!_arMeshManager.enabled)
            {
                _arMeshManager.density = newDensity;
                return;
            }

            OnDisableMeshGeneration();
            _arMeshManager.density = newDensity;
            OnEnableMeshGeneration();
        }

        private void OnEnableMeshGeneration()
        {
            _arMeshManager.enabled = true;
        }

        private void OnDisableMeshGeneration()
        {
            _arMeshManager.enabled = false;
        }

        private void SendGeneratedMeshes()
        {
            var meshFilters = new List<MeshFilter>();
            foreach (MeshFilter meshFilter in _arMeshManager.meshes)
            {
                meshFilters.Add(meshFilter);
            }

            EventManager.AppEvent.OnMeshesSent.RaiseEvent(meshFilters);
        }

        private void OnDestroyAllGeneratedMeshes()
        {
            _arMeshManager.DestroyAllMeshes();
        }
    }
}

