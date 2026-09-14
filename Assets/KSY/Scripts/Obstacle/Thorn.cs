using System;
using SeungyungLib.Md.Body.Core;

using System.Collections.Generic;
using UnityEngine;

namespace KSY.Obstacle
{
    public class Thorn : MonoBehaviour
    {
        //임시 대미지 직렬화 필드
        [SerializeField] private int damage;

        private readonly float damageInterval = 0.1f;
        private readonly List<IDamageable> _contactDamageableList = new List<IDamageable>();

        private void LateUpdate()
        {
            if (_contactDamageableList.Count != 0)
                foreach (var damageable in _contactDamageableList)
                    damageable?.ApplyDamage(new DamageContext(damage, gameObject));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            IDamageable damageable = other.gameObject.GetComponentInChildren<IDamageable>(true);

            if (damageable != null)
            {
                _contactDamageableList.Add(damageable);
                damageable?.ApplyDamage(new DamageContext(damage, gameObject));
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            IDamageable damageable = other.gameObject.GetComponentInChildren<IDamageable>(true);

            if (damageable != null)
            {
                int index = _contactDamageableList.IndexOf(damageable);
                if (index != -1) _contactDamageableList.RemoveAt(index);
            }
        }

        private void OnDestroy()
        {
            _contactDamageableList.Clear();
        }
    }
}
