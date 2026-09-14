using System;
using SeungyungLib.Core.ManagerSystem;
using SeungyungLib.Core.ReadOnlyAttribute;

using UnityEngine;

namespace KSY.StressSystem
{
    public class StressManagement : MonoBehaviour, IManagement
    {
        public delegate void StressHandler(int currentStress);
        
        [field: SerializeField, ReadOnly] public int CurrentStress { get; private set; }
        [field: SerializeField] public int MaxStress { get; private set; }
        [field: SerializeField] public int MinStress { get; private set; }

        public event StressHandler OnStressGained;
        public event StressHandler OnStressMaxed;
        public event StressHandler OnStressMinimised;

        private StressPostProcessController _effectController;
        
        public void Initialize()
        {
            this._effectController = GetComponentInChildren<StressPostProcessController>();
            
            OnStressGained += _effectController.SetStressRatio;
        }

        private void OnDestroy()
        {
            OnStressGained -= _effectController.SetStressRatio;
        }

        public void SetStress(int plusValue)
        {
            int prev = CurrentStress;
            CurrentStress = Mathf.Clamp(CurrentStress + plusValue, MinStress, MaxStress);
            
            if (prev < CurrentStress)
                OnStressGained?.Invoke(CurrentStress);
            else if (CurrentStress == MaxStress)
                OnStressMaxed?.Invoke(CurrentStress);
            else if (CurrentStress == MinStress)
                OnStressMinimised?.Invoke(CurrentStress);
        }
    }
}