using System;
using DG.Tweening;
using UnityEngine;

namespace ProjectC.Armory
{
    public class AreaDamager : MonoBehaviour
    {
        private float _duration;

        public void Damage(float radius, int damage = 100, Action onComplete = null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var hitCollider in hitColliders)
            {
                if(!hitCollider.TryGetComponent(out IDamageable damageable)) continue;
                damageable.GetDamage(damage);
                print("Added Damage");
            }

            DOVirtual.DelayedCall(0.5f, () => onComplete?.Invoke());
        }
    }
}