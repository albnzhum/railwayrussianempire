using UnityEngine;
using Zenject;
using Railway.UI;
using Railway.Input;
using Railway.Gameplay;
using Railway.Shop.UI;
using Railway.Gameplay.UI;
using Railway.Components;
using Railway.Settings;

namespace Railway.SceneManagement
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("UI")] 
        public UIManager uiManager;
        public UIHud uiHud;
        public UIShop uiShop;
        public UIPause uiPause;
        public UIPopup uiPopup;
        public UISettingsController uiSettingsController;
        public UIResources uiResources;

        [Header("Gameplay")] 
        public InputReader inputReader;
        public GameStateSO gameStateSO;
        public MissionInitializer missionInitializer;
        public SettingsSO settingsSO;

        public override void InstallBindings()
        {
            // UI
            Container.Bind<UIManager>().FromInstance(uiManager).AsSingle();
            Container.Bind<UIHud>().FromInstance(uiHud).AsSingle();
            Container.Bind<UIShop>().FromInstance(uiShop).AsSingle();
            Container.Bind<UIPause>().FromInstance(uiPause).AsSingle();
            Container.Bind<UIPopup>().FromInstance(uiPopup).AsSingle();
            Container.Bind<UISettingsController>().FromInstance(uiSettingsController).AsSingle();
            Container.Bind<UIResources>().FromInstance(uiResources).AsSingle();

            // Gameplay
            Container.Bind<InputReader>().FromInstance(inputReader).AsSingle();
            Container.Bind<GameStateSO>().FromInstance(gameStateSO).AsSingle();
            Container.Bind<MissionInitializer>().FromInstance(missionInitializer).AsSingle();
            Container.Bind<SettingsSO>().FromInstance(settingsSO).AsSingle();
        }
    }
}
