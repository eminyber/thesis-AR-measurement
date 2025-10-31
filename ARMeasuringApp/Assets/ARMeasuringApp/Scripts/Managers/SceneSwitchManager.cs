using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Util.Enums;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARMeasurementApp.Scripts.Managers
{
    public class SceneSwitchManager : MonoBehaviour
    {
        void OnEnable()
        {
            EventManager.AppEvent.SwitchScene.AddListener(OnSceneSwitch);
        }

        void OnDisable()
        {
            EventManager.AppEvent.SwitchScene.RemoveListener(OnSceneSwitch);
        }

        private void OnSceneSwitch(ARMeasurementAppScene newSceneIndex)
        {
            SceneManager.LoadScene((int) newSceneIndex);
        }
    }
}

