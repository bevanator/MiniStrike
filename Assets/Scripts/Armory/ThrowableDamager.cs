using System;
using System.Collections;
using DG.Tweening;
using ProjectC.Constants;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectC.Armory
{
    public class ThrowableDamager : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        [SerializeField] private AreaDamager _areaDamager;
        [SerializeField] private DamageType m_DamageType;
        [SerializeField] private float m_Delay;
        [SerializeField] private float m_Duration;
        [SerializeField] private Transform m_Mesh;
        [SerializeField] private float m_Radius = 5f;
        [SerializeField] private ParticleSystem m_ExplosionParticle;
        
        private float _timer;
        private bool _isComplete;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Propel(Quaternion rotation, float power)
        {
            transform.rotation = rotation;
            _rigidbody.AddForce(power * 10f * transform.forward, ForceMode.Impulse);
            if(m_DamageType is not DamageType.TimeBased) return;
            InflictDamageAfterDelay(m_Delay);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(m_DamageType is not DamageType.CollisionBased) return;
            InflictDamageOverTime(m_Duration);
        }

        private void InflictDamageOverTime(float duration)
        {
            m_Mesh.gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;
            transform.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
            _areaDamager.enabled = true;
            m_ExplosionParticle.gameObject.SetActive(true);
            m_ExplosionParticle.Play();
            SetFXDuration(duration);
            StartCoroutine(nameof(DamageInIntervals));
            DOVirtual.DelayedCall(duration + 0.6f, Hide);
        }

        private IEnumerator DamageInIntervals()
        {
            while (!_isComplete)
            {
                _areaDamager.Damage(2f, -15);
                yield return new WaitForSeconds(0.6f);
            }
        }


        private void SetFXDuration(float duration)
        {
            // int initialCount = m_ExplosionParticle.emission.burstCount;
            // ParticleSystem.EmissionModule explosionParticleEmission = m_ExplosionParticle.emission;
            // ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[explosionParticleEmission.burstCount];
            // DOVirtual.Float(initialCount, 0f, 1f, (value) =>
            //         {
            //             bursts[0].minCount = bursts[0].maxCount = (short)value;
            //             explosionParticleEmission.SetBursts(bursts);
            //         })
            //     .SetDelay(duration);
            DOVirtual.DelayedCall(duration, () =>
            {
                m_ExplosionParticle.Stop();
            });
        }
        
        
        

        private void InflictDamageAfterDelay(float delay)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                InflictDamage(m_Radius);
                m_ExplosionParticle.gameObject.SetActive(true);
                m_ExplosionParticle.Play();
            });
            DOVirtual.DelayedCall(delay + 0.8f, Hide);
        }

        private void Hide()
        {
            _isComplete = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void InflictDamage(float radius)
        {
            _areaDamager.enabled = true;
            m_Mesh.gameObject.SetActive(false);
            _rigidbody.isKinematic = false;
            _rigidbody.freezeRotation = true;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.rotation = quaternion.identity;
            _areaDamager.Damage(radius, -100);
        }
    }

    public enum DamageType
    {
        TimeBased,
        CollisionBased
    }
}