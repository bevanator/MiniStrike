using System;
using ProjectC.Constants;
using UnityEngine;

namespace Enemy
{
    public class EnemyBase : MonoBehaviour
    {
        public static event Action EnemyDeadEvent;
        protected virtual void Die()
        {
            EnemyDeadEvent?.Invoke();
        }
        protected bool CanSeePlayer(float viewDistance, LayerMask mask)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

            for (int i = 0; i < 5; i++)
            {
                Vector3 leftDir = i * -0.1f * transform.right;
                Vector3 rightDir = i * 0.1f * transform.right;
                if (RayHitPlayer(leftDir + transform.forward)) return true;
                if (RayHitPlayer(rightDir + transform.forward)) return true;
            }
            
            bool RayHitPlayer(Vector3 direction)
            {
                ray = new Ray(transform.position + Vector3.up, direction);
                if (Physics.Raycast(ray, out hit, viewDistance, mask))
                {
                    return hit.collider.gameObject.CompareTag(GameTags.PLAYER);
                }

                return false;
            }
 
            return false;
        }
    }
    
    public class EnemySoldierData
    {
        public int Health;
        public Color BaseColor;
        public float Speed;
        public int WeaponIndex;
        public float DamageMultiplier;
        public float FireRateFactor;
    }

}