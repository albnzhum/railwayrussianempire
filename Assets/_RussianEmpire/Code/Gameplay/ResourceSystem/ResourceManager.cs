using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Railway.Components;
using Railway.Gameplay.EconomySystem;
using Railway.Gameplay.UI;

namespace Railway.Gameplay.ResourceSystem
{
    [Serializable]
    public class WorkforceData
    {
        public string Id;
        public string Profession;
        public float Experience;
        public float Efficiency;
        public float Salary;
        public float Satisfaction;
        public Dictionary<string, float> Skills;
    }

    [Serializable]
    public class ResourceStorage
    {
        public Dictionary<ResourceType, float> CurrentAmount;
        public Dictionary<ResourceType, float> MaxCapacity;
        public Dictionary<ResourceType, float> ConsumptionRate;
        public Dictionary<ResourceType, float> ProductionRate;
    }

    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance;

        [SerializeField] private EconomyManager _economyManager;
        
        private Dictionary<string, WorkforceData> _workforce = new Dictionary<string, WorkforceData>();
        private Dictionary<string, ResourceStorage> _storages = new Dictionary<string, ResourceStorage>();
        private Dictionary<ResourceType, float> _globalResources = new Dictionary<ResourceType, float>();

        private void Awake()
        {
            Instance = this;
            InitializeGlobalResources();
        }

        private void InitializeGlobalResources()
        {
            _globalResources = new Dictionary<ResourceType, float>
            {
                { ResourceType.Gold, 1000f },
                { ResourceType.Church, 500f },
                { ResourceType.Fuel, 800f },
                { ResourceType.Workers, 1000f }
            };
        }

        public string HireWorker(string profession, float initialSalary)
        {
            string workerId = Guid.NewGuid().ToString();
            var worker = new WorkforceData
            {
                Id = workerId,
                Profession = profession,
                Experience = 0f,
                Efficiency = 1f,
                Salary = initialSalary,
                Satisfaction = 1f,
                Skills = new Dictionary<string, float>()
            };

            _workforce.Add(workerId, worker);
            return workerId;
        }

        public void UpdateWorkerEfficiency(string workerId)
        {
            if (!_workforce.TryGetValue(workerId, out var worker))
                return;

            // Базовая эффективность зависит от опыта
            float experienceBonus = Mathf.Lerp(0f, 0.5f, worker.Experience / 10f);
            
            // Учитываем удовлетворенность работой
            float satisfactionMultiplier = Mathf.Lerp(0.5f, 1.2f, worker.Satisfaction);
            
            worker.Efficiency = (1f + experienceBonus) * satisfactionMultiplier;
        }

        public void UpdateWorkerSatisfaction(string workerId)
        {
            if (!_workforce.TryGetValue(workerId, out var worker))
                return;

            // Рассчитываем справедливую зарплату на основе опыта и профессии
            float fairSalary = _economyManager.CalculateWorkerSalary(
                worker.Profession,
                worker.Experience,
                worker.Efficiency
            );

            // Удовлетворенность зависит от соотношения текущей и справедливой зарплаты
            float salaryFactor = Mathf.Clamp01(worker.Salary / fairSalary);
            
            worker.Satisfaction = Mathf.Lerp(0.5f, 1f, salaryFactor);
        }

        public void AddStorage(string locationId, Dictionary<ResourceType, float> capacities)
        {
            var storage = new ResourceStorage
            {
                CurrentAmount = new Dictionary<ResourceType, float>(),
                MaxCapacity = new Dictionary<ResourceType, float>(capacities),
                ConsumptionRate = new Dictionary<ResourceType, float>(),
                ProductionRate = new Dictionary<ResourceType, float>()
            };

            foreach (var resource in capacities.Keys)
            {
                storage.CurrentAmount[resource] = 0f;
                storage.ConsumptionRate[resource] = 0f;
                storage.ProductionRate[resource] = 0f;
            }

            _storages[locationId] = storage;
        }

        public bool TryAddResource(string locationId, ResourceType type, float amount)
        {
            if (!_storages.TryGetValue(locationId, out var storage))
                return false;

            if (!storage.CurrentAmount.ContainsKey(type) || 
                !storage.MaxCapacity.ContainsKey(type))
                return false;

            float newAmount = storage.CurrentAmount[type] + amount;
            if (newAmount > storage.MaxCapacity[type])
                return false;

            storage.CurrentAmount[type] = newAmount;
            return true;
        }

        public bool TryRemoveResource(string locationId, ResourceType type, float amount)
        {
            if (!_storages.TryGetValue(locationId, out var storage))
                return false;

            if (!storage.CurrentAmount.ContainsKey(type) || 
                storage.CurrentAmount[type] < amount)
                return false;

            storage.CurrentAmount[type] -= amount;
            return true;
        }

        public void UpdateResourceProduction(string locationId)
        {
            if (!_storages.TryGetValue(locationId, out var storage))
                return;

            foreach (var resource in storage.ProductionRate.Keys.ToList())
            {
                float production = storage.ProductionRate[resource] * Time.deltaTime;
                if (production > 0)
                {
                    TryAddResource(locationId, resource, production);
                }
            }
        }

        public void UpdateResourceConsumption(string locationId)
        {
            if (!_storages.TryGetValue(locationId, out var storage))
                return;

            foreach (var resource in storage.ConsumptionRate.Keys.ToList())
            {
                float consumption = storage.ConsumptionRate[resource] * Time.deltaTime;
                if (consumption > 0)
                {
                    TryRemoveResource(locationId, resource, consumption);
                }
            }
        }

        private void Update()
        {
            foreach (var locationId in _storages.Keys)
            {
                UpdateResourceProduction(locationId);
                UpdateResourceConsumption(locationId);
            }

            foreach (var workerId in _workforce.Keys)
            {
                UpdateWorkerEfficiency(workerId);
                UpdateWorkerSatisfaction(workerId);
            }
        }

        public Dictionary<ResourceType, float> GetResourceStatus(string locationId)
        {
            if (!_storages.TryGetValue(locationId, out var storage))
                return null;

            return new Dictionary<ResourceType, float>(storage.CurrentAmount);
        }

        public List<WorkforceData> GetWorkforceStatus()
        {
            return new List<WorkforceData>(_workforce.Values);
        }
    }
} 