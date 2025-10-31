using ARMeasurementApp.Scripts.Events;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace ARMeasurementApp.Scripts.UI.Managers
{
    public class ConfidenceValueSliderVisibilityManager : MonoBehaviour
    {
        [SerializeField] TMP_Text _sliderText;
        [SerializeField] Slider _slider;

        void OnEnable()
        {
            EventManager.UIEvent.ConfidenceValueFilteringStateChanged.AddListener(ToggleVisibility);   
        }

        void OnDisable()
        {
            EventManager.UIEvent.ConfidenceValueFilteringStateChanged.RemoveListener(ToggleVisibility);
        }

        void Start()
        {
            if (_sliderText == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in ConfidenceValueSliderVisibilityManager: _sliderText is null");
                enabled = false;
                return;
            }

            if (_sliderText == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in ConfidenceValueSliderVisibilityManager: _slider is null");
                enabled = false;
                return;
            }
        }

        private void ToggleVisibility(bool isConfidenceValueFilteringActive)
        {
            _sliderText.gameObject.SetActive(isConfidenceValueFilteringActive);
            _slider.gameObject.SetActive(isConfidenceValueFilteringActive);
        }
    }
}

