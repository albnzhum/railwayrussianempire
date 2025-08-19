using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Railway.SceneManagement
{
    public class GameSceneSO : ScriptableObject
    {
        public GameSceneType sceneType;
        public AssetReference sceneReference;

        public enum GameSceneType
        {
            //Playable scenes
            Location, 
            Menu, 

            //Special scenes
            Initialisation,
            PersistentManagers,
            Gameplay
        }
    }
}