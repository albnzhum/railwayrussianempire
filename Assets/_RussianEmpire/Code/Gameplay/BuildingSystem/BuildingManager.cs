using System;
using System.Collections.Generic;
using UnityEngine;
using Railway.Components;
using Railway.Gameplay.UI;

namespace Railway.Gameplay.BuildingSystem
{
    [Serializable]
    public class BuildingRequest
    {
        public string BuildingType;
        public Vector3 Position;
        public Quaternion Rotation;
        public Dictionary<ResourceType, float> RequiredResources;
        public float ConstructionTime;
        public float Progress;
    }

    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance;

        [SerializeField] private ResourcesManager _resourcesManager;
        [SerializeField] private RailwayObjectManager _railwayObjectManager;
        
        private Dictionary<string, BuildingRequest> _activeConstructions = new Dictionary<string, BuildingRequest>();
        private Dictionary<string, Dictionary<ResourceType, float>> _buildingCosts;

        private void Awake()
        {
            Instance = this;
            //InitializeBuildingCosts();
        }

      /*  private void InitializeBuildingCosts()
        {
            _buildingCosts = new Dictionary<string, Dictionary<ResourceType, float>>
            {
                {
                    "Station", new Dictionary<ResourceType, float>
                    {
                        { ResourceType.Money, 1000f },
                        { ResourceType.Wood, 100f },
                        { ResourceType.Stone, 200f },
                        { ResourceType.Iron, 50f }
                    }
                },
                {
                    "Track", new Dictionary<ResourceType, float>
                    {
                        { ResourceType.Money, 100f },
                        { ResourceType.Wood, 10f },
                        { ResourceType.Iron, 20f }
                    }
                }
            };
        }*/

        public bool CanAffordConstruction(string buildingType)
        {
            if (!_buildingCosts.ContainsKey(buildingType)) return false;

            foreach (var resource in _buildingCosts[buildingType])
            {
               /* var currentAmount = _resourcesManager.GetCurrentAmount(resource.Key);
                if (currentAmount < resource.Value) return false;*/
            }

            return true;
        }

        public string StartConstruction(string buildingType, Vector3 position, Quaternion rotation)
        {
            if (!CanAffordConstruction(buildingType)) return null;

            string constructionId = Guid.NewGuid().ToString();
            var request = new BuildingRequest
            {
                BuildingType = buildingType,
                Position = position,
                Rotation = rotation,
                RequiredResources = new Dictionary<ResourceType, float>(_buildingCosts[buildingType]),
                ConstructionTime = CalculateConstructionTime(buildingType, position),
                Progress = 0f
            };

            // Списываем ресурсы
            foreach (var resource in request.RequiredResources)
            {
                _resourcesManager.Spend(resource.Key, resource.Value);
            }

            _activeConstructions.Add(constructionId, request);
            return constructionId;
        }

        private float CalculateConstructionTime(string buildingType, Vector3 position)
        {
            float baseTime = buildingType == "Station" ? 60f : 30f; // базовое время в секундах
            float terrainMultiplier = CalculateTerrainTimeMultiplier(position);
            float weatherMultiplier = CalculateWeatherTimeMultiplier();
            
            return baseTime * terrainMultiplier * weatherMultiplier;
        }

        private float CalculateTerrainTimeMultiplier(Vector3 position)
        {
            // Здесь будет логика расчета влияния местности на время строительства
            return 1f;
        }

        private float CalculateWeatherTimeMultiplier()
        {
            // Здесь будет логика расчета влияния погоды на время строительства
            return 1f;
        }

        private void Update()
        {
            var completedConstructions = new List<string>();

            foreach (var construction in _activeConstructions)
            {
                construction.Value.Progress += Time.deltaTime / construction.Value.ConstructionTime;

                if (construction.Value.Progress >= 1f)
                {
                    CompleteBuildingConstruction(construction.Key, construction.Value);
                    completedConstructions.Add(construction.Key);
                }
            }

            foreach (var id in completedConstructions)
            {
                _activeConstructions.Remove(id);
            }
        }

        private void CompleteBuildingConstruction(string constructionId, BuildingRequest request)
        {
            string prefabPath = $"Prefabs/Buildings/{request.BuildingType}";
            if (_railwayObjectManager.TryCreateObject(prefabPath, request.Position, request.Rotation, out string objectId))
            {
                // Здесь можно добавить дополнительную логику после успешного строительства
                Debug.Log($"Construction completed: {request.BuildingType} (ID: {objectId})");
            }
        }
    }
} 