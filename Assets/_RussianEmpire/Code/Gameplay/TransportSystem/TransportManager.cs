using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Railway.Components;
using Railway.Gameplay.EconomySystem;

namespace Railway.Gameplay.TransportSystem
{
    [Serializable]
    public class CargoData
    {
        public string Type;
        public float Amount;
        public float Weight;
        public float Value;
        public bool IsPerishable;
        public float TimeToSpoil;
    }

    [Serializable]
    public class TransportRoute
    {
        public string Id;
        public string StartStationId;
        public string EndStationId;
        public List<string> IntermediateStationIds;
        public List<Vector3> PathPoints;
        public float TotalDistance;
        public float AverageSpeed;
        public Dictionary<string, float> TrackConditions;
    }

    public class TransportManager : MonoBehaviour
    {
        public static TransportManager Instance;

        [SerializeField] private EconomyManager _economyManager;
        [SerializeField] private RailwayObjectManager _railwayObjectManager;
        
        private Dictionary<string, TransportRoute> _activeRoutes = new Dictionary<string, TransportRoute>();
        private Dictionary<string, List<CargoData>> _stationStorage = new Dictionary<string, List<CargoData>>();
        private Dictionary<string, float> _cargoSpeedModifiers = new Dictionary<string, float>();

        private void Awake()
        {
            Instance = this;
            InitializeCargoModifiers();
        }

        private void InitializeCargoModifiers()
        {
            _cargoSpeedModifiers = new Dictionary<string, float>
            {
                { "Coal", 0.9f },    // Тяжелый груз, медленнее
                { "Wood", 0.95f },   // Относительно тяжелый
                { "Mail", 1.1f },    // Легкий груз, быстрее
                { "Passengers", 1.2f } // Пассажирские перевозки самые быстрые
            };
        }

        public string CreateRoute(string startStationId, string endStationId, List<string> intermediateStations)
        {
            string routeId = Guid.NewGuid().ToString();
            var route = new TransportRoute
            {
                Id = routeId,
                StartStationId = startStationId,
                EndStationId = endStationId,
                IntermediateStationIds = intermediateStations,
                PathPoints = CalculateRoutePath(startStationId, endStationId, intermediateStations),
                TrackConditions = new Dictionary<string, float>()
            };

            route.TotalDistance = CalculateRouteDistance(route.PathPoints);
            route.AverageSpeed = CalculateAverageSpeed(route);
            
            _activeRoutes.Add(routeId, route);
            return routeId;
        }

        private List<Vector3> CalculateRoutePath(string startId, string endId, List<string> intermediateIds)
        {
            var path = new List<Vector3>();
            // В реальной игре здесь будет использоваться алгоритм поиска пути
            // с учетом существующих железнодорожных путей
            return path;
        }

        private float CalculateRouteDistance(List<Vector3> pathPoints)
        {
            float distance = 0f;
            for (int i = 1; i < pathPoints.Count; i++)
            {
                distance += Vector3.Distance(pathPoints[i - 1], pathPoints[i]);
            }
            return distance;
        }

        private float CalculateAverageSpeed(TransportRoute route)
        {
            float baseSpeed = 60f; // км/ч
            float totalConditionModifier = 0f;

            foreach (var condition in route.TrackConditions.Values)
            {
                totalConditionModifier += condition;
            }

            float averageCondition = totalConditionModifier / route.TrackConditions.Count;
            return baseSpeed * averageCondition;
        }

        public float CalculateDeliveryTime(string routeId, string cargoType)
        {
            if (!_activeRoutes.TryGetValue(routeId, out var route))
                return -1f;

            float speedModifier = _cargoSpeedModifiers.GetValueOrDefault(cargoType, 1f);
            float effectiveSpeed = route.AverageSpeed * speedModifier;
            
            return route.TotalDistance / effectiveSpeed;
        }

        public void AddCargoToStation(string stationId, CargoData cargo)
        {
            if (!_stationStorage.ContainsKey(stationId))
            {
                _stationStorage[stationId] = new List<CargoData>();
            }

            _stationStorage[stationId].Add(cargo);
        }

        public bool TryLoadCargo(string stationId, string cargoType, float amount, out CargoData loadedCargo)
        {
            loadedCargo = null;
            if (!_stationStorage.ContainsKey(stationId))
                return false;

            var availableCargo = _stationStorage[stationId]
                .FirstOrDefault(c => c.Type == cargoType && c.Amount >= amount);

            if (availableCargo == null)
                return false;

            loadedCargo = new CargoData
            {
                Type = availableCargo.Type,
                Amount = amount,
                Weight = availableCargo.Weight * (amount / availableCargo.Amount),
                Value = availableCargo.Value * (amount / availableCargo.Amount),
                IsPerishable = availableCargo.IsPerishable,
                TimeToSpoil = availableCargo.TimeToSpoil
            };

            availableCargo.Amount -= amount;
            if (availableCargo.Amount <= 0)
            {
                _stationStorage[stationId].Remove(availableCargo);
            }

            return true;
        }

        public void UpdateRouteConditions(string routeId, Dictionary<string, float> newConditions)
        {
            if (!_activeRoutes.TryGetValue(routeId, out var route))
                return;

            route.TrackConditions = newConditions;
            route.AverageSpeed = CalculateAverageSpeed(route);
        }

        public float CalculateTransportationCost(string routeId, CargoData cargo)
        {
            if (!_activeRoutes.TryGetValue(routeId, out var route))
                return 0f;

            float baseCost = cargo.Value * 0.1f; // 10% от стоимости груза
            float distanceMultiplier = route.TotalDistance * 0.001f; // 0.1% за километр
            float conditionMultiplier = route.TrackConditions.Values.Average();

            return baseCost * distanceMultiplier * conditionMultiplier;
        }

        public void SimulateTransportation(string routeId, CargoData cargo)
        {
            if (!_activeRoutes.TryGetValue(routeId, out var route))
                return;

            float deliveryTime = CalculateDeliveryTime(routeId, cargo.Type);
            float transportationCost = CalculateTransportationCost(routeId, cargo);
            
            // Рассчитываем доход от перевозки
            float income = _economyManager.CalculateTransportationIncome(
                route.TotalDistance,
                cargo.Amount,
                cargo.Type,
                route.TrackConditions.Values.Average()
            );

            // Учитываем расходы
            float profit = income - transportationCost;
            
            // Добавляем прибыль в экономику
            _economyManager.AddIncome(profit);
        }
    }
} 