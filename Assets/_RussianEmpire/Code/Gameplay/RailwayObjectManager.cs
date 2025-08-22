using System;
using System.Collections.Generic;
using UnityEngine;
using Railway.Components;
using Railway.Gameplay.UI;

namespace Railway.Gameplay
{
    [Serializable]
    public class RailwayObject
    {
        public string Id;
        public Vector3 Position;
        public Quaternion Rotation;
        public float Condition;
        public float Efficiency;
        public ResourceType[] RequiredResources;
        public float[] ResourceAmounts;
    }

    public class RailwayObjectManager : MonoBehaviour
    {
        public static RailwayObjectManager Instance;

        [SerializeField] private ResourcesManager _resourcesManager;
        
        private Dictionary<string, RailwayObject> _railwayObjects = new Dictionary<string, RailwayObject>();
        private Dictionary<string, GameObject> _objectInstances = new Dictionary<string, GameObject>();

        private void Awake()
        {
            Instance = this;
        }

        public bool TryCreateObject(string prefabPath, Vector3 position, Quaternion rotation, out string objectId)
        {
            objectId = Guid.NewGuid().ToString();
            
            var prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null) return false;

            var instance = Instantiate(prefab, position, rotation);
            var railwayObject = new RailwayObject
            {
                Id = objectId,
                Position = position,
                Rotation = rotation,
                Condition = 100f,
                Efficiency = 100f
            };

            _railwayObjects.Add(objectId, railwayObject);
            _objectInstances.Add(objectId, instance);

            return true;
        }

        public void UpdateObjectCondition(string objectId, float newCondition)
        {
            if (!_railwayObjects.ContainsKey(objectId)) return;
            
            _railwayObjects[objectId].Condition = Mathf.Clamp(newCondition, 0f, 100f);
            UpdateObjectEfficiency(objectId);
        }

        private void UpdateObjectEfficiency(string objectId)
        {
            if (!_railwayObjects.ContainsKey(objectId)) return;

            var obj = _railwayObjects[objectId];
            // Эффективность зависит от состояния объекта и окружающих условий
            float terrainMultiplier = CalculateTerrainEfficiencyMultiplier(obj.Position);
            float weatherMultiplier = CalculateWeatherEfficiencyMultiplier();
            
            obj.Efficiency = (obj.Condition / 100f) * terrainMultiplier * weatherMultiplier;
        }

        private float CalculateTerrainEfficiencyMultiplier(Vector3 position)
        {
            // Здесь будет логика расчета влияния местности на эффективность
            return 1f;
        }

        private float CalculateWeatherEfficiencyMultiplier()
        {
            // Здесь будет логика расчета влияния погоды на эффективность
            return 1f;
        }

        public void DestroyObject(string objectId)
        {
            if (!_railwayObjects.ContainsKey(objectId)) return;

            if (_objectInstances.TryGetValue(objectId, out var instance))
            {
                Destroy(instance);
                _objectInstances.Remove(objectId);
            }

            _railwayObjects.Remove(objectId);
        }
    }
} 