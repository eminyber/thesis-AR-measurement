using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Interfaces;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.TextUpdateHandlers
{
    public class InfoTextUpdateHandler : MonoBehaviour, ITextUpdateHandler<string>
    {
        [SerializeField] TextMeshProUGUI _infoTextMesh;
        void OnEnable()
        {

            EventManager.UIEvent.UpdateInfoText.AddListener(UpdateText);
        }

        void OnDisable()
        {
            EventManager.UIEvent.UpdateInfoText.RemoveListener(UpdateText);
        }

        void Start()
        {
            if (_infoTextMesh != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in InfoTextUpdateHandler: _infoTextMesh is null");
            enabled = false;
        }


        public void UpdateText(string value)
        {
            _infoTextMesh.text = value;
        }
    }
}

