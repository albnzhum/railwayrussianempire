using System;
using System.Collections.Generic;
using Railway.Events;
using Railway.Input;
using Railway.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace Railway.UI
{
    [Serializable]
    public enum SettingFieldType
    {
        Volume_SFX,
        Volume_Music,
        Volume_Master,
        Resolution,
        Fullscreen,
        ShadowDistance,
        AntiAliasing,
        ShadowQuality
    }

    [Serializable]
    public class SettingTab
    {
        public SettingsType settingTabType;
    }

    [Serializable]
    public class SettingField
    {
        public SettingsType settingTabType;
        public SettingFieldType settingFieldType;
    }

    public enum SettingsType
    {
        General,
        Gameplay,
        Graphics,
        Audio,
    }

    public class UISettingsController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private UIGraphicsSettingsComponent _graphicsComponent;
        [SerializeField] private UIAudioSettingsComponent _audioComponent;
        [SerializeField] private UIControlsSettingsComponent _controlsComponent;
        [SerializeField] private List<SettingsType> _settingsTabTypes = new List<SettingsType>();
        [SerializeField] private SettingsSO _currentSettings;
        [SerializeField] private VoidEventChannelSO SaveSettings;

        private SettingsType _selectedTab = SettingsType.Audio;

        public UnityAction Closed;

        private void OnEnable()
        {
            _inputReader.MenuCloseEvent += CloseScreen;
        }

        private void OnDisable()
        {
            _inputReader.MenuCloseEvent -= CloseScreen;
        }

        public void CloseScreen()
        {
            Closed.Invoke();
        }
    }
}