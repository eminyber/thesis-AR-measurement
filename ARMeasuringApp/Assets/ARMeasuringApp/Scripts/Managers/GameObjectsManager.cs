using ARMeasurementApp.Scripts.Events;

using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Managers
{
    public class GameObjectsManager : MonoBehaviour
    {
        [SerializeField] GameObject _objectPrefab; 

        private List<GameObject> _objects = new List<GameObject>(); 

        public int ObjectCount => _objects.Count;
       
        void OnDisable()
        {
            if(_objects.Count > 0)
                DeleteAllObjects();
        }

        void Start()
        {
            if (_objectPrefab != null) return;

            EventManager.AppEvent.LogError.RaiseEvent("Error in GameObjectsManager: _objectPrefab is null");
            enabled = false;
        }

        public GameObject CreateNewObject(Vector3 position, Quaternion rotation)
        {
            if (!enabled) return null;

            GameObject newObject = Instantiate(_objectPrefab, position, rotation);
            if (newObject == null)
            {
                EventManager.AppEvent.LogError.RaiseEvent("Error in GameObjectsManager -> CreateNewObject: A new GameObject could not be initialized");
                return null; 
            }
            _objects.Add(newObject);

            return newObject;
        }

        public GameObject GetObject(int index)
        {
            if (!enabled || index < 0 || index >= _objects.Count) return null;

            return _objects[index];
        }

        public void DeleteObject(int index)
        {
            if (!enabled || index < 0 || index >= _objects.Count) return;

            Destroy(_objects[index]); 
            _objects.RemoveAt(index);
        }

        public void DeleteLastObject()
        {
            DeleteObject(_objects.Count - 1); 
        }

        public void DeleteAllObjects()
        {
            if (!enabled) return;

            foreach (GameObject obj in _objects)
            {
                Destroy(obj);
            }
            _objects.Clear();
        }
    }
}
