using System;
using System.Collections;
using System.Collections.Generic;
using Railway.Events;
using UnityEngine;

namespace Railway.SceneManagement
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _sceneToLoad;
        [SerializeField] private bool _showLoadScreen = default;

        [Header("Broadcasting on")] [SerializeField]
        private LoadEventChannelSO _loadScene = default;

        [Header("Listening to")] [SerializeField]
        private VoidEventChannelSO _onNewGameButton = default;

        [SerializeField] private VoidEventChannelSO _onContinueButton = default;

        private bool _hasSaveData;

        private void Start()
        {
            _onNewGameButton.OnEventRaised += StartGameInit;
        }

        private void OnDestroy()
        {
            _onNewGameButton.OnEventRaised -= StartGameInit;
        }

        private void StartGameInit()
        {
            _hasSaveData = false;
            _loadScene.RaiseEvent(_sceneToLoad, _showLoadScreen);
        }
    }
}