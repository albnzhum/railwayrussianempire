using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Railway.Components;
using Railway.Gameplay.EconomySystem;

namespace Railway.Gameplay.ResearchSystem
{
    [Serializable]
    public class ResearchInstitute
    {
        public string Id;
        public string Name;
        public float Efficiency;
        public float FundingLevel;
        public List<string> SpecializedFields;
        public Dictionary<string, float> ResearchProgress;
    }

    [Serializable]
    public class Technology
    {
        public string Id;
        public string Name;
        public string Description;
        public float ResearchCost;
        public float ResearchProgress;
        public List<string> Prerequisites;
        public Dictionary<string, float> Bonuses;
        public int YearAvailable;
    }

    public class ResearchManager : MonoBehaviour
    {
        public static ResearchManager Instance;

        [SerializeField] private EconomyManager _economyManager;
        [SerializeField] private TextAsset _technologiesJson;
        
        private Dictionary<string, ResearchInstitute> _institutes = new Dictionary<string, ResearchInstitute>();
        private Dictionary<string, Technology> _technologies = new Dictionary<string, Technology>();
        private List<string> _researchedTechnologies = new List<string>();
        private int _currentYear;

        private void Awake()
        {
            Instance = this;
            LoadTechnologies();
        }

        private void LoadTechnologies()
        {
            // В реальной игре здесь будет загрузка технологий из JSON
            _technologies = new Dictionary<string, Technology>
            {
                {
                    "steam_engine_mk2", new Technology
                    {
                        Id = "steam_engine_mk2",
                        Name = "Улучшенный паровой двигатель",
                        Description = "Повышает эффективность локомотивов на 20%",
                        ResearchCost = 1000f,
                        ResearchProgress = 0f,
                        Prerequisites = new List<string>(),
                        Bonuses = new Dictionary<string, float>
                        {
                            { "locomotive_efficiency", 1.2f },
                            { "fuel_consumption", 0.9f }
                        },
                        YearAvailable = 1865
                    }
                },
                {
                    "steel_rails", new Technology
                    {
                        Id = "steel_rails",
                        Name = "Стальные рельсы",
                        Description = "Увеличивает скорость движения и снижает износ путей",
                        ResearchCost = 1500f,
                        ResearchProgress = 0f,
                        Prerequisites = new List<string> { "steam_engine_mk2" },
                        Bonuses = new Dictionary<string, float>
                        {
                            { "track_durability", 1.3f },
                            { "max_speed", 1.25f }
                        },
                        YearAvailable = 1870
                    }
                }
            };
        }

        public string CreateInstitute(string name, List<string> specializations, float initialFunding)
        {
            string instituteId = Guid.NewGuid().ToString();
            var institute = new ResearchInstitute
            {
                Id = instituteId,
                Name = name,
                Efficiency = 1f,
                FundingLevel = initialFunding,
                SpecializedFields = specializations,
                ResearchProgress = new Dictionary<string, float>()
            };

            _institutes.Add(instituteId, institute);
            return instituteId;
        }

        public void UpdateInstituteFunding(string instituteId, float newFunding)
        {
            if (!_institutes.TryGetValue(instituteId, out var institute))
                return;

            float oldFunding = institute.FundingLevel;
            institute.FundingLevel = newFunding;

            // Эффективность института зависит от изменения финансирования
            if (newFunding < oldFunding)
            {
                institute.Efficiency *= Mathf.Lerp(0.8f, 1f, newFunding / oldFunding);
            }
            else if (newFunding > oldFunding)
            {
                institute.Efficiency *= Mathf.Lerp(1f, 1.2f, oldFunding / newFunding);
            }
        }

        public bool StartResearch(string instituteId, string technologyId)
        {
            if (!_institutes.TryGetValue(instituteId, out var institute) ||
                !_technologies.TryGetValue(technologyId, out var technology))
                return false;

            // Проверяем доступность технологии по году
            if (_currentYear < technology.YearAvailable)
                return false;

            // Проверяем наличие предпосылок
            foreach (var prerequisite in technology.Prerequisites)
            {
                if (!_researchedTechnologies.Contains(prerequisite))
                    return false;
            }

            institute.ResearchProgress[technologyId] = 0f;
            return true;
        }

        public void UpdateResearch()
        {
            foreach (var institute in _institutes.Values)
            {
                foreach (var research in institute.ResearchProgress.ToList())
                {
                    if (!_technologies.TryGetValue(research.Key, out var technology))
                        continue;

                    // Базовый прогресс зависит от финансирования и эффективности
                    float baseProgress = institute.FundingLevel * institute.Efficiency * Time.deltaTime;
                    
                    // Если институт специализируется в данной области, добавляем бонус
                    if (institute.SpecializedFields.Any(field => technology.Name.ToLower().Contains(field.ToLower())))
                    {
                        baseProgress *= 1.5f;
                    }

                    institute.ResearchProgress[research.Key] += baseProgress;

                    // Проверяем завершение исследования
                    if (institute.ResearchProgress[research.Key] >= technology.ResearchCost)
                    {
                        CompleteResearch(research.Key);
                        institute.ResearchProgress.Remove(research.Key);
                    }
                }
            }
        }

        private void CompleteResearch(string technologyId)
        {
            if (!_technologies.TryGetValue(technologyId, out var technology))
                return;

            _researchedTechnologies.Add(technologyId);

            // Применяем бонусы технологии
            ApplyTechnologyBonuses(technology);
        }

        private void ApplyTechnologyBonuses(Technology technology)
        {
            // Здесь будет логика применения бонусов технологии
            // Например, увеличение эффективности определенных объектов
        }

        public float GetResearchProgress(string instituteId, string technologyId)
        {
            if (!_institutes.TryGetValue(instituteId, out var institute) ||
                !_technologies.TryGetValue(technologyId, out var technology))
                return 0f;

            if (!institute.ResearchProgress.TryGetValue(technologyId, out float progress))
                return 0f;

            return progress / technology.ResearchCost;
        }

        public List<Technology> GetAvailableTechnologies()
        {
            return _technologies.Values
                .Where(t => t.YearAvailable <= _currentYear &&
                           t.Prerequisites.All(p => _researchedTechnologies.Contains(p)))
                .ToList();
        }

        public void UpdateYear(int newYear)
        {
            _currentYear = newYear;
        }
    }
} 