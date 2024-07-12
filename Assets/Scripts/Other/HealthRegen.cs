using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Other
{
    public class HealthRegen : MonoBehaviour
    {
        private Health _health;
        private Shootable _shootable;
        private bool _willRegen;
        private float _timer;
        [SerializeField] private float m_AmountPerSecond = 1;
        [SerializeField] private float m_RegenStartDelay = 2;
        private Tween _delayTween;
        private bool _isActive = true;

        private void Awake()
        {
            _shootable = GetComponent<Shootable>();
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _shootable.DamageReceived += OnReceivedDamageEvent;
            _health.HealthIsEmptyEvent += OnHealthIsEmptyEvent;
        }

        private void OnDisable()
        {
            _shootable.DamageReceived -= OnReceivedDamageEvent;
            _health.HealthIsEmptyEvent -= OnHealthIsEmptyEvent;
        }

        private void OnHealthIsEmptyEvent()
        {
            _isActive = false;
        }

        private void OnReceivedDamageEvent()
        {
            _willRegen = false;
            _delayTween.Kill();
            _delayTween = DOVirtual.DelayedCall(m_RegenStartDelay, () => _willRegen = true);
        }

        private void Update()
        {
            if(!_isActive) return;
            if(!_willRegen) return;
            if (_health.CurrentHealth >= _health.MaxHealth) return;
            _timer += Time.deltaTime;
            if (!(_timer > 1f / m_AmountPerSecond)) return;
            _timer = 0;
            _health.SetHealth(1);
        }
    }
}