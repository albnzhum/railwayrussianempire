using Railway.Gameplay;
using TGS;
using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<NeighborFinder>().FromNew().AsSingle().NonLazy();
        Container.Bind<TerrainGridSystem>().FromInstance(TerrainGridSystem.Instance);
    }
}