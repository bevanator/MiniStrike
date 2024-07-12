using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Health : MonoBehaviour
{
    [ShowInInspector] private float _currentHealth;

    [field:SerializeField] private float m_MaxHealth = 100f;

    public float MaxHealth => m_MaxHealth;

    public float CurrentHealth => _currentHealth;

    public event Action HealthIsEmptyEvent;
    public event Action<float> HealthModifiedEvent;

    private void Awake()
    {
        _currentHealth = m_MaxHealth;
    }

    public void SetHealth(float value)
    {
        _currentHealth += value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, m_MaxHealth);
        HealthModifiedEvent?.Invoke(value);
        if (_currentHealth <= 0)
        {
            HealthIsEmptyEvent?.Invoke();
        }
    }
}
