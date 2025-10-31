using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ARMeasurementApp.Scripts.Events;

namespace ARMeasurementApp.Scripts.Managers
{
    public class TextMeshProsManager : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textMeshProPrefab;

        private List<TextMeshPro> _texts = new List<TextMeshPro>();

        public int TextCount => _texts.Count;

        void OnDisable()
        {
            if (_texts.Count > 0)
                DeleteAllTextMeshes();
        }

        void Start()
        {
            if (_textMeshProPrefab != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in TextMeshProsManager: _textMeshProPrefab is null");
            enabled = false;
        }

        public TextMeshPro CreateNewTextMesh(Vector3 position, Quaternion rotation)
        {
            if (!enabled) return null;

            TextMeshPro newTextMesh = Instantiate(_textMeshProPrefab, position, rotation);
            if (newTextMesh == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in TextMeshProsManager -> CreateNewTextMesh: A new TextMeshPro could not be initialized");
                return null;
            }
            _texts.Add(newTextMesh);

            return newTextMesh;
        }

        public TextMeshPro GetTextMesh(int index)
        {
            if (!enabled || index < 0 || index >= _texts.Count) return null;

            return _texts[index];
        }

        public void DeleteTextMesh(int index)
        {
            if (!enabled || index < 0 || index >= _texts.Count) return;

            Destroy(_texts[index]);
            _texts.RemoveAt(index);
        }

        public void DeleteLastTextMesh()
        {
            DeleteTextMesh(_texts.Count - 1);
        }

        public void DeleteAllTextMeshes()
        {
            if (!enabled) return;

            foreach (TextMeshPro text in _texts)
            {
                Destroy(text);
            }
            _texts.Clear();
        }
    }
}
