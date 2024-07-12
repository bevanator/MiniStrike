using System;
using DG.Tweening;
using Enemy;
using Other;
using PlayerController;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectC
{
    public class Player : MonoBehaviour
    {
        public static Player Instance;
        private Health _health;
        public static event Action OnPlayerDeath;

        private AnimationHandler _animationHandler;
        private CharacterController _characterController;
        
        private void Awake()
        {
            Instance = this;
            _health = GetComponent<Health>();
            _characterController = GetComponent<CharacterController>();
            _animationHandler = GetComponent<AnimationHandler>();
        }

        private void OnEnable()
        {
            _health.HealthIsEmptyEvent += OnHealthEmptyEvent;
        }
        private void OnDisable()
        {
            _health.HealthIsEmptyEvent -= OnHealthEmptyEvent;
        }

        private void OnHealthEmptyEvent()
        {
            Die();
            OnPlayerDeath?.Invoke();
        }
        
        [Button]
        private void Die()
        {
            _animationHandler.PlayDeathAnim();
            _characterController.enabled = false;
            InputController.DisableInput();
            // DOVirtual.DelayedCall(2f, () => gameObject.SetActive(false));
        }
    }
}