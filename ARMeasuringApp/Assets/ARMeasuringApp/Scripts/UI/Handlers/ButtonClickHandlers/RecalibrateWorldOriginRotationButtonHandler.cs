using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace ARMeasurementApp.Scripts.UI.Handlers.ButtonClickHandlers
{
    public class RecalibrateWorldOriginRotationButtonHandler : MonoBehaviour, IButtonClickHandler
    {
        [SerializeField] ARSession _arSession;
        [SerializeField] GameObject _originPrefab;

        private GameObject _origin;

        private const float SCALE_VALUE = 0.25f;

        void Start()
        {
            if (_arSession == null) 
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in RecalibrateWorldOriginRotation: _arSession is null");
                enabled = false;
                return;
            }

            if (_originPrefab == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in RecalibrateWorldOriginRotation: _originPrefab is null");
                enabled = false;
                return;
            }

            _origin = Instantiate(_originPrefab, _arSession.transform.position, _arSession.transform.rotation);
            _origin.transform.localScale = new Vector3(SCALE_VALUE, SCALE_VALUE, SCALE_VALUE);
        }

        public void OnButtonClick()
        {
            if (!enabled) return;

            _arSession.Reset();
        }
    }
}

