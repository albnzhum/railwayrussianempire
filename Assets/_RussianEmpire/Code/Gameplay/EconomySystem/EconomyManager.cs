using UnityEngine;
using System;
using System.Collections.Generic;
using Railway.Components;
using Railway.Gameplay.UI;

namespace Railway.Gameplay.EconomySystem
{
    [Serializable]
    public class HistoricalPrices
    {
        public int Year;
        public Dictionary<ResourceType, float> BasePrices;
        public Dictionary<string, float> WorkerSalaries; // по профессиям
        public float InflationRate;
    }

    [Serializable]
    public class MaintenanceCost
    {
        public float BaseCost;
        public float WorkforceCost;
        public float MaterialsCost;
        public float RepairCost;
    }

    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance;

        [SerializeField] private ResourcesManager _resourcesManager;
        [SerializeField] private TextAsset _historicalDataJson;

        private Dictionary<int, HistoricalPrices> _historicalPrices = new Dictionary<int, HistoricalPrices>();
        private Dictionary<string, MaintenanceCost> _maintenanceCosts = new Dictionary<string, MaintenanceCost>();
        private float _currentBalance;
        private int _currentYear;

        private void Awake()
        {
            Instance = this;
            LoadHistoricalData();
            InitializeMaintenanceCosts();
        }

        private void LoadHistoricalData()
        {
            // В реальной игре здесь будет загрузка исторических данных из JSON
            _historicalPrices = new Dictionary<int, HistoricalPrices>();
            // Пример данных для 1860 года
            _historicalPrices[1860] = new HistoricalPrices
            {
                Year = 1860,
                BasePrices = new Dictionary<ResourceType, float>
                {
                   /* { ResourceType.Wood, 10f },
                    { ResourceType.Iron, 50f },
                    { ResourceType.Coal, 15f },
                    { ResourceType.Stone, 8f }*/
                },
                WorkerSalaries = new Dictionary<string, float>
                {
                    { "Engineer", 100f },
                    { "Worker", 30f },
                    { "Conductor", 45f }
                },
                InflationRate = 0.02f
            };
        }

        private void InitializeMaintenanceCosts()
        {
            _maintenanceCosts = new Dictionary<string, MaintenanceCost>
            {
                {
                    "Track", new MaintenanceCost
                    {
                        BaseCost = 10f,
                        WorkforceCost = 5f,
                        MaterialsCost = 3f,
                        RepairCost = 2f
                    }
                },
                {
                    "Station", new MaintenanceCost
                    {
                        BaseCost = 50f,
                        WorkforceCost = 25f,
                        MaterialsCost = 15f,
                        RepairCost = 10f
                    }
                }
            };
        }

        public float CalculateTransportationIncome(float distance, float cargo, string cargoType, float efficiency)
        {
            // Базовая формула: (расстояние * груз * базовая_цена * эффективность)
            float baseIncome = distance * cargo * GetCargoBasePrice(cargoType);
            
            // Учитываем эффективность перевозки
            float effectiveIncome = baseIncome * efficiency;
            
            // Применяем исторические модификаторы
            float historicalMultiplier = GetHistoricalPriceMultiplier();
            
            return effectiveIncome * historicalMultiplier;
        }

        public float CalculateMaintenanceCost(string objectType, float condition)
        {
            if (!_maintenanceCosts.TryGetValue(objectType, out var cost))
                return 0f;

            // Базовая стоимость обслуживания
            float baseCost = cost.BaseCost;
            
            // Увеличиваем стоимость при плохом состоянии
            float conditionMultiplier = Mathf.Lerp(2f, 1f, condition / 100f);
            
            // Учитываем стоимость рабочей силы и материалов
            float totalCost = (baseCost + cost.WorkforceCost + cost.MaterialsCost) * conditionMultiplier;
            
            // Если состояние плохое, добавляем стоимость ремонта
            if (condition < 50f)
            {
                totalCost += cost.RepairCost * (1f - condition / 50f);
            }

            return totalCost * GetHistoricalPriceMultiplier();
        }

        public float CalculateWorkerSalary(string profession, float experience, float efficiency)
        {
            if (!_historicalPrices[_currentYear].WorkerSalaries.TryGetValue(profession, out float baseSalary))
                return 0f;

            // Учитываем опыт работника
            float experienceMultiplier = Mathf.Lerp(1f, 1.5f, Mathf.Clamp01(experience / 10f));
            
            // Учитываем эффективность работы
            float efficiencyMultiplier = Mathf.Lerp(0.8f, 1.2f, efficiency);
            
            return baseSalary * experienceMultiplier * efficiencyMultiplier;
        }

        private float GetCargoBasePrice(string cargoType)
        {
            // В реальной игре здесь будет система динамических цен
            return 10f;
        }

        private float GetHistoricalPriceMultiplier()
        {
            if (!_historicalPrices.TryGetValue(_currentYear, out var prices))
                return 1f;

            return 1f + prices.InflationRate * (_currentYear - 1860);
        }

        public void UpdateYear(int newYear)
        {
            _currentYear = newYear;
            // Здесь можно добавить логику обновления цен при смене года
        }

        public void AddIncome(float amount)
        {
            _currentBalance += amount;
            _resourcesManager.Add(ResourceType.Gold, amount);
        }

        public bool TrySpendMoney(float amount)
        {
            if (_currentBalance < amount)
                return false;

            _currentBalance -= amount;
            _resourcesManager.Spend(ResourceType.Gold, amount);
            return true;
        }
    }
} 