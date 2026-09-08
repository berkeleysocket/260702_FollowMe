using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeungyungLib.Core.BaseCollider
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask whatIsCheck;
        
        public int ContactCount => _contactObjects.Count;
        
        private readonly Dictionary<CollisionOption, Action<CollisionContext>> _actions = new Dictionary<CollisionOption, Action<CollisionContext>>();
        private readonly List<GameObject> _contactObjects = new List<GameObject>();

        public void RegisterAction(CollisionOption moduleOption, Action<CollisionContext> action)
        {
            _actions[moduleOption] = action;
        }
        
        private void InvokeEvent(CollisionOption moduleOption, CollisionContext context)
        {
            if (_actions.TryGetValue(moduleOption, out var action))
                action?.Invoke(context);
        }

        private bool IsTargetLayer(int layer, out LayerMask targetMask)
        {
            targetMask = 1 << layer;
            return (targetMask.value & whatIsCheck.value) != 0;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            GameObject otherGO = other.gameObject;

            if (IsTargetLayer(otherGO.layer, out var targetMask))
            {
                if (!_contactObjects.Contains(otherGO))
                    _contactObjects.Add(otherGO);

                CollisionContext context = new CollisionContext(otherGO, targetMask);
                InvokeEvent(CollisionOption.Collision | CollisionOption.Enter, context);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            GameObject otherGO = other.gameObject;

            if (IsTargetLayer(otherGO.layer, out var targetMask))
            {
                CollisionContext context = new CollisionContext(otherGO, targetMask);
                InvokeEvent(CollisionOption.Collision | CollisionOption.Stay, context);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            GameObject otherGO = other.gameObject;

            if (IsTargetLayer(otherGO.layer, out var targetMask))
            {
                _contactObjects.Remove(otherGO);
                CollisionContext context = new CollisionContext(otherGO, targetMask);
                InvokeEvent(CollisionOption.Collision | CollisionOption.Exit, context);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GameObject otherGO = other.gameObject;

            if (IsTargetLayer(otherGO.layer, out var targetMask))
            {
                if (!_contactObjects.Contains(otherGO))
                    _contactObjects.Add(otherGO);
                
                CollisionContext context = new CollisionContext(otherGO, targetMask);
                InvokeEvent(CollisionOption.Trigger | CollisionOption.Enter, context);
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            GameObject otherGO = other.gameObject;

            if (IsTargetLayer(otherGO.layer, out var targetMask))
            {
                CollisionContext context = new CollisionContext(otherGO, targetMask);
                InvokeEvent(CollisionOption.Trigger | CollisionOption.Stay, context);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            GameObject otherGO = other.gameObject;

            if (IsTargetLayer(otherGO.layer, out var targetMask))
            {
                _contactObjects.Remove(otherGO);
                CollisionContext context = new CollisionContext(otherGO, targetMask);
                InvokeEvent(CollisionOption.Trigger | CollisionOption.Exit, context);
            }
        }
    }
}