using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Enum;
using SeungyungLib.ModuleSystem.Core;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    [RequireComponent(typeof(Collider2D))]
    public class ColliderModule : MonoBehaviour, IColliderModule
    {
        [SerializeField] private LayerMask whatIsCheck;
        
        public int ContactCount => _contactObjects.Count;
        
        private readonly Dictionary<ColliderModuleOption, Action<GameObject>> _actions = new Dictionary<ColliderModuleOption, Action<GameObject>>();
        private readonly List<GameObject> _contactObjects = new List<GameObject>();

        public void RegisterAction(ColliderModuleOption moduleOption, Action<GameObject> action)
        {
            _actions[moduleOption] = action;
        }
        
        private void InvokeEvent(ColliderModuleOption moduleOption, GameObject other)
        {
            if (_actions.TryGetValue(moduleOption, out var action))
                action?.Invoke(other);
        }

        private bool IsTargetLayer(int layer)
        {
            return ((1 << layer) & whatIsCheck.value) != 0;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (IsTargetLayer(other.gameObject.layer))
            {
                _contactObjects.Add(other.gameObject);
                InvokeEvent(ColliderModuleOption.Collision | ColliderModuleOption.Enter, other.gameObject);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (IsTargetLayer(other.gameObject.layer))
                InvokeEvent(ColliderModuleOption.Collision | ColliderModuleOption.Stay, other.gameObject);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (IsTargetLayer(other.gameObject.layer))
            {
                _contactObjects.Remove(other.gameObject);
                InvokeEvent(ColliderModuleOption.Collision | ColliderModuleOption.Exit, other.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsTargetLayer(other.gameObject.layer))
            {
                _contactObjects.Add(other.gameObject);   
                InvokeEvent(ColliderModuleOption.Trigger | ColliderModuleOption.Enter, other.gameObject);
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsTargetLayer(other.gameObject.layer))
                InvokeEvent(ColliderModuleOption.Trigger | ColliderModuleOption.Stay, other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsTargetLayer(other.gameObject.layer))
            {
                _contactObjects.Remove(other.gameObject);
                InvokeEvent(ColliderModuleOption.Trigger | ColliderModuleOption.Exit, other.gameObject);
            }
        }
    }
}