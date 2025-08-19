using System;
using R3;
using UnityEngine;

namespace Railway.Shop.Data
{
    [Serializable]
    public enum WorkerType
    {
        Land,
        Water,
        Complex
    }

    [CreateAssetMenu(fileName = "New Worker Item", menuName = "Shop Data/Worker")]
    public class WorkerSO : ShopItem
    {
        [SerializeField] private ReactiveProperty<int> _count;
        [SerializeField] private ReactiveProperty<float> _salary;
        [SerializeField] private ReactiveProperty<float> _provision;
        [SerializeField] private ReactiveProperty<float> _satisfaction;
        [SerializeField] private ReactiveProperty<float> _speedBuilding;
        [SerializeField] private WorkerType _workerType;

        public ReactiveProperty<int> Count => _count;
        public ReactiveProperty<float> Salary => _salary;
        public ReactiveProperty<float> Provision => _provision;
        public ReactiveProperty<float> Satisfaction => _satisfaction;
        public ReactiveProperty<float> SpeedBuilding => _speedBuilding;
        public WorkerType WorkerType => _workerType;
    }
}