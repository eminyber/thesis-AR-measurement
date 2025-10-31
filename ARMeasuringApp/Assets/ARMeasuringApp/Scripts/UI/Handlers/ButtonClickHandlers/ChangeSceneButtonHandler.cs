using ARMeasurementApp.Scripts.Interfaces;
using ARMeasurementApp.Scripts.Events;
using ARMeasurementApp.Scripts.Util.Enums;

using UnityEngine;

namespace ARMeasurementApp.Scripts.UI.Handlers.ButtonClickHandlers
{
    public class ChangeSceneButtonHandler : MonoBehaviour, IButtonClickHandler
    {
        [SerializeField] ARMeasurementAppScene _newScene;

        public void OnButtonClick()
        {
            // Reset the current measurement system if one returns to the start menu scene
            if (_newScene == ARMeasurementAppScene.StartMenu)
                EventManager.ButtonClickEvent.ResetCurrentScene.RaiseEvent();

            EventManager.AppEvent.SwitchScene.RaiseEvent(_newScene);
        }
    }
}

