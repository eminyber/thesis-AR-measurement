using ARMeasurementApp.Scripts.Events;

using TMPro;
using UnityEngine;

namespace ARMeasurementApp.Scripts.Utiil
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _logTextMesh;

        [SerializeField] int _maxNumberOfTextLines = 12;

        void OnEnable()
        {
            _logTextMesh.raycastTarget = false;

            EventManager.AppEvent.Log.AddListener(LogInfo);
            EventManager.AppEvent.LogWarning.AddListener(LogWarning);
            EventManager.AppEvent.LogError.AddListener(LogError);
            EventManager.AppEvent.ClearLog.AddListener(ClearLogText);
        }

        void OnDisable()
        {
            EventManager.AppEvent.Log.RemoveListener(LogInfo);
            EventManager.AppEvent.LogWarning.RemoveListener(LogWarning);
            EventManager.AppEvent.LogError.RemoveListener(LogError);
            EventManager.AppEvent.ClearLog.RemoveListener(ClearLogText);
        }

        void Start()
        {
            if (_logTextMesh == null)
                enabled = false;

            if (_maxNumberOfTextLines < 1)
                enabled = false;
        }

        private void LogInfo(string logMessage)
        {
            ClearLineIfOverflow();
            _logTextMesh.text += $"<color=\"white\">{logMessage}</color>\n";
        }

        private void LogWarning(string logMessage)
        {
            ClearLineIfOverflow();
            _logTextMesh.text += $"<color=\"yellow\">{logMessage}</color>\n";
        }

        private void LogError(string logMessage)
        {
            ClearLineIfOverflow();
            _logTextMesh.text += $"<color=\"red\">{logMessage}</color>\n";
        }

        private void ClearLogText()
        {
            _logTextMesh.text = string.Empty;
        }

        private void ClearLineIfOverflow()
        {
            if (_logTextMesh.text.Split('\n').Length > _maxNumberOfTextLines)
            {
                _logTextMesh.text = string.Empty;
            }
        }
    }
}
