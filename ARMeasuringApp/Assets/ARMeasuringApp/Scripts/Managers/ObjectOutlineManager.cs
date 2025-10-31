using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Events;

using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Managers
{
    public class ObjectOutlineManager : MonoBehaviour
    {
        [SerializeField] GameObjectsManager _outlinePointManager;
        [SerializeField] LineRenderer _lineRendererPrefab;

        [SerializeField] float _lineWidth = 0.030f;

        private LineRenderer _lineRenderer;
        
        private LineRenderController _lineRenderController = new LineRenderController();

        private bool _isOutlineComplete = false;
        private float _maxOutlineHeightInWorld = -Mathf.Infinity;

        public int OutlineVerticesCount => _outlinePointManager.ObjectCount;

        void Start()
        {
            if (_outlinePointManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager: _outlinePointManager is null");
                enabled = false;
                return;
            }

            if (_lineRendererPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager: _lineRendererPrefab is null");
                enabled = false;
                return;
            }

            if (_lineWidth <= 0)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager: Line width is set to zero or lower");
                enabled = false;
                return;
            }

            _lineRenderer = Instantiate(_lineRendererPrefab);

            ApplyLineSettings();
        }

        public GameObject AddOutlinePoint(Pose pose)
        {
            if (!enabled || pose == null) return null;

            if (_isOutlineComplete)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in OutlineObjectManager -> AddOutlinePoint: The outline is already complete");
                return null;
            }
      
            GameObject newOutlinePoint = _outlinePointManager.CreateNewObject(pose.position, pose.rotation);
            if (newOutlinePoint == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager -> AddOutlinePoint: A new outline point could not be initialized");
                return null;
            }

            if (pose.position.y > _maxOutlineHeightInWorld)
                _maxOutlineHeightInWorld = pose.position.y;

            return newOutlinePoint;
        }

        public void ConstructObjectOutline()
        {
            if (!enabled) return;

            if (_isOutlineComplete)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in OutlineObjectManager -> ConstructObjectOutline: The outline is already complete");
                return;
            }

            if (_outlinePointManager.ObjectCount < 3)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in OutlineObjectManager -> ConstructObjectOutline: The outline must consists of atleast three points");
                return;
            }

            for (int i = 0; i < _outlinePointManager.ObjectCount; i++)
            {
                GameObject outlinePoint = _outlinePointManager.GetObject(i);
                _lineRenderController.AddPointToLine(_lineRenderer, outlinePoint.transform.position);
            }

            _isOutlineComplete = true;
        }

        public void ClearObjectOutline()
        {
            if (!enabled) return;

            _outlinePointManager.DeleteAllObjects();
            _lineRenderController.ClearLines(_lineRenderer);

            _isOutlineComplete = false;
            _maxOutlineHeightInWorld = -Mathf.Infinity;
        }

        public List<Vector2> GetOutlineVerticesIn2D()
        {
            if (!enabled) return null;

            var outlineVertices = new List<Vector2>();
            for (int i = 0; i < _outlinePointManager.ObjectCount; i++)
            {
                Vector3 outlinePointPosition = _outlinePointManager.GetObject(i).transform.position;
                outlineVertices.Add(new Vector2(outlinePointPosition.x, outlinePointPosition.z));
            }

            return outlineVertices;
        }

        public float GetOutlineHeightInWorld()
        {
            if (!enabled) return default;

            return _maxOutlineHeightInWorld;
        }

        public Vector3 GetOutlineCenter()
        {
            if (!enabled || !_isOutlineComplete) return default;

            Vector3 firstDirectionVector = (_outlinePointManager.GetObject(2).transform.position - _outlinePointManager.GetObject(0).transform.position).normalized;
            float firstDistance = Vector3.Distance(_outlinePointManager.GetObject(2).transform.position, _outlinePointManager.GetObject(0).transform.position);

            Vector3 secondDirectionVector = (_outlinePointManager.GetObject(3).transform.position - _outlinePointManager.GetObject(1).transform.position).normalized;
            float secondDistance = Vector3.Distance(_outlinePointManager.GetObject(3).transform.position, _outlinePointManager.GetObject(1).transform.position);

            Vector3 firstCenter = _outlinePointManager.GetObject(0).transform.position + (firstDirectionVector * (firstDistance / 2));
            Vector3 secondCenter = _outlinePointManager.GetObject(1).transform.position + (secondDirectionVector * (secondDistance / 2));

            return (firstCenter + secondCenter) / 2;
        }

        public bool IsOutlineCompleted()
        {
            if (!enabled) return false;

            return _isOutlineComplete;
        }

        private void ApplyLineSettings()
        {
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;

            _lineRenderer.loop = true;
        }
    }
}
