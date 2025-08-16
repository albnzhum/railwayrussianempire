using UnityEngine;
using System.Collections.Generic;
using Railway.Components;

namespace Railway.Gameplay.EfficiencySystem
{
    public class TerrainEfficiencyData
    {
        public float BaseMultiplier;
        public float HeightMultiplier;
        public float SlopeMultiplier;
        public float SoilQualityMultiplier;
    }

    public class WeatherEfficiencyData
    {
        public float TemperatureMultiplier;
        public float PrecipitationMultiplier;
        public float WindMultiplier;
    }

    public class EfficiencyCalculator : MonoBehaviour
    {
        public static EfficiencyCalculator Instance;

        [SerializeField] private AnimationCurve _temperatureEfficiencyCurve;
        [SerializeField] private AnimationCurve _precipitationEfficiencyCurve;
        [SerializeField] private AnimationCurve _windEfficiencyCurve;
        [SerializeField] private AnimationCurve _slopeEfficiencyCurve;

        private Dictionary<string, TerrainEfficiencyData> _terrainEfficiencyCache = new Dictionary<string, TerrainEfficiencyData>();
        private WeatherEfficiencyData _currentWeatherData;

        private void Awake()
        {
            Instance = this;
            InitializeWeatherData();
        }

        private void InitializeWeatherData()
        {
            _currentWeatherData = new WeatherEfficiencyData
            {
                TemperatureMultiplier = 1f,
                PrecipitationMultiplier = 1f,
                WindMultiplier = 1f
            };
        }

        public float CalculateObjectEfficiency(RailwayObject obj, Vector3 position)
        {
            var terrainData = GetTerrainEfficiencyData(position);
            float terrainEfficiency = CalculateTerrainEfficiency(terrainData);
            float weatherEfficiency = CalculateWeatherEfficiency(_currentWeatherData);
            float conditionEfficiency = obj.Condition / 100f;

            // Базовая формула эффективности
            float baseEfficiency = terrainEfficiency * weatherEfficiency * conditionEfficiency;

            // Применяем исторические модификаторы (например, технологический уровень эпохи)
            float historicalMultiplier = GetHistoricalMultiplier();
            
            // Учитываем влияние рабочей силы
            float laborEfficiency = CalculateLaborEfficiency();

            return baseEfficiency * historicalMultiplier * laborEfficiency;
        }

        private TerrainEfficiencyData GetTerrainEfficiencyData(Vector3 position)
        {
            string key = $"{position.x:F1},{position.y:F1},{position.z:F1}";
            
            if (_terrainEfficiencyCache.TryGetValue(key, out var cachedData))
                return cachedData;

            var data = new TerrainEfficiencyData
            {
                BaseMultiplier = 1f,
                HeightMultiplier = CalculateHeightMultiplier(position.y),
                SlopeMultiplier = CalculateSlopeMultiplier(position),
                SoilQualityMultiplier = CalculateSoilQualityMultiplier(position)
            };

            _terrainEfficiencyCache[key] = data;
            return data;
        }

        private float CalculateTerrainEfficiency(TerrainEfficiencyData data)
        {
            return data.BaseMultiplier * 
                   data.HeightMultiplier * 
                   data.SlopeMultiplier * 
                   data.SoilQualityMultiplier;
        }

        private float CalculateWeatherEfficiency(WeatherEfficiencyData data)
        {
            return data.TemperatureMultiplier * 
                   data.PrecipitationMultiplier * 
                   data.WindMultiplier;
        }

        private float CalculateHeightMultiplier(float height)
        {
            // Высота влияет на эффективность (сложнее строить и обслуживать на большой высоте)
            float normalizedHeight = Mathf.Clamp01(height / 1000f); // нормализуем до 1000 метров
            return Mathf.Lerp(1f, 0.5f, normalizedHeight);
        }

        private float CalculateSlopeMultiplier(Vector3 position)
        {
            // Используем физику Unity для определения уклона местности
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up, Vector3.down, out hit))
            {
                float slope = Vector3.Angle(hit.normal, Vector3.up);
                return _slopeEfficiencyCurve.Evaluate(slope / 90f);
            }
            return 1f;
        }

        private float CalculateSoilQualityMultiplier(Vector3 position)
        {
            // В реальной игре здесь будет использоваться карта качества почвы
            return 1f;
        }

        private float GetHistoricalMultiplier()
        {
            // Здесь будет логика получения исторического модификатора
            // в зависимости от текущего года в игре
            return 1f;
        }

        private float CalculateLaborEfficiency()
        {
            // Здесь будет логика расчета эффективности рабочей силы
            // с учетом условий труда, оплаты и т.д.
            return 1f;
        }

        public void UpdateWeatherConditions(float temperature, float precipitation, float windSpeed)
        {
            _currentWeatherData.TemperatureMultiplier = _temperatureEfficiencyCurve.Evaluate(
                Mathf.InverseLerp(-40f, 40f, temperature)
            );
            
            _currentWeatherData.PrecipitationMultiplier = _precipitationEfficiencyCurve.Evaluate(
                Mathf.InverseLerp(0f, 100f, precipitation)
            );
            
            _currentWeatherData.WindMultiplier = _windEfficiencyCurve.Evaluate(
                Mathf.InverseLerp(0f, 30f, windSpeed)
            );
        }
    }
} 