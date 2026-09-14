using System;
using SeungyungLib.Md.Body.Core;
using UnityEngine;

namespace KSY.Obstacle
{
    public class Thorn : MonoBehaviour
    {
        //임시 대미지 직렬화 필드
        [SerializeField] private int damage;

        private void OnCollisionEnter2D(Collision2D other)
        {
            IDamageable damageable = other.gameObject.GetComponentInChildren<IDamageable>();
            damageable?.ApplyDamage(new DamageContext(damage, gameObject));
        }
    }
}
