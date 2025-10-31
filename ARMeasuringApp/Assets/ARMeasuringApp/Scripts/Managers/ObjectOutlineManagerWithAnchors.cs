using ARMeasurementApp.Scripts.Controllers;
using ARMeasurementApp.Scripts.Events;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMeasurementApp.Scripts.Managers
{
    public class ObjectOutlineManagerWithAnchors : MonoBehaviour
    {
        [SerializeField] LineRenderer _lineRendererPrefab;
        [SerializeField] ARAnchorManager _arAnchorManager;

        [SerializeField] float _lineWidth = 0.030f;

        private LineRenderer _lineRenderer;
        private LineRenderController _lineRenderController;

        private List<ARAnchor> _anchors;

        private bool _isOutlineComplete;
        private float _maxOutlineHeightInWorld;

        void OnEnable()
        {
            EventManager.AppEvent.OnARAnchorRemoved.AddListener(HandleARAnchorRemoved);
        }

        void OnDisable()
        {
            EventManager.AppEvent.OnARAnchorRemoved.RemoveListener(HandleARAnchorRemoved);
        }

        void Start() 
        {
            if (_lineRendererPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager: _lineRendererPrefab is null");
                enabled = false;
                return;
            }

            if (_arAnchorManager == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectmanager: _arAnchorManager is null");
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
            _lineRenderController = new LineRenderController();

            _anchors = new List<ARAnchor>();

            _isOutlineComplete = false;
            _maxOutlineHeightInWorld = -Mathf.Infinity;

            ApplyLineSettings();
        }

        private void HandleARAnchorRemoved(ARAnchor anchor)
        {
            if (anchor == null) return;

            Destroy(anchor);
        }

        public ARAnchor AddOutlinePointToPlane(ARPlane plane, Pose pose)
        {
            if (!enabled || pose == null) return null;

            if (_isOutlineComplete)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in OutlineObjectManager -> AddOutlinePointToPlanes: The outline is already complete");
                return null;
            }

            ARAnchor newAnchor = _arAnchorManager.AttachAnchor(plane, pose);
            if (newAnchor == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager -> AddOutlinePointToPlane: A new anchor could not be attached to the plane");
                return null;
            }
            _anchors.Add(newAnchor);

            if (pose.position.y > _maxOutlineHeightInWorld)
                _maxOutlineHeightInWorld = pose.position.y;

            return newAnchor;
        }

        public void ConstructObjectOutline()
        {
            if (!enabled) return;

            if (_isOutlineComplete)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in OutlineObjectManager -> ConstructObjectOutline: The outline is already complete");
                return;
            }

            if (_anchors.Count < 3)
            {
                EventManager.AppEvent.LogWarning.RaiseEvent("Warning in OutlineObjectManager -> ConstructObjectOutline: The outline must consists of atleast three points");
                return;
            }

            for (int i = 0; i < _anchors.Count; i++)
            {
                _lineRenderController.AddPointToLine(_lineRenderer, _anchors[i].pose.position);
            }

            _isOutlineComplete = true;
        }

        public List<Vector2> GetOutlineVerticesIn2D()
        {
            if (!enabled || !_isOutlineComplete) return default;

            List<Vector2> outlineVertices = new List<Vector2>();
            for (int i = 0; i < _anchors.Count; i++)
            {
                outlineVertices.Add(new Vector2(_anchors[i].pose.position.x, _anchors[i].pose.position.z));
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

            Vector3 firstDirectionVector = (_anchors[2].pose.position - _anchors[0].pose.position).normalized;
            float firstDistance = Vector3.Distance(_anchors[2].pose.position, _anchors[0].pose.position);

            Vector3 secondDirectionVector = (_anchors[3].pose.position - _anchors[1].pose.position).normalized;
            float secondDistance = Vector3.Distance(_anchors[3].pose.position, _anchors[1].pose.position);

            Vector3 firstCenter = _anchors[0].pose.position + (firstDirectionVector * (firstDistance / 2));
            Vector3 secondCenter = _anchors[1].pose.position + (secondDirectionVector * (secondDistance / 2));

            return (firstCenter + secondCenter) / 2;
        }

        public bool IsOutlineCompleted()
        {
            if (!enabled) return false;

            return _isOutlineComplete;
        }

        public void ClearObjectOutline()
        {
            if (!enabled) return;

            for (int i = 0; i < _anchors.Count; i++)
            {
                var success = _arAnchorManager.TryRemoveAnchor(_anchors[i]);
                if (!success)
                {
                    EventManager.AppEvent.LogError.RaiseEvent("Error in OutlineObjectManager -> ClearObjectOutline: Acnhor: " + i.ToString() + " could not be deleted.");
                    continue;
                }
            }
            _anchors.Clear();

            _isOutlineComplete = false;
            _maxOutlineHeightInWorld = -Mathf.Infinity;

            _lineRenderController.ClearLines(_lineRenderer);
        }

        private void ApplyLineSettings()
        {
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;

            _lineRenderer.loop = true;
        }
    }
}

