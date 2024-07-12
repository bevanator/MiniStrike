using System;
using PlayerController;
using ProjectC.Armory;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Shootable : MonoBehaviour, IDamageable
{
    public event Action DamageReceived;
    public event Action OnDamageableDeath;
    [SerializeField] private Health m_Health;
    [SerializeField] private ParticleSystem m_ImpactParticle;
    [SerializeField] private DamagerType m_DamagerType;
    
    private AttackController _attackController;

    private void Awake()
    {
        _attackController = FindFirstObjectByType<AttackController>();
        m_ImpactParticle = GetComponentInChildren<ParticleSystem>(); //todo: change later
        m_Health = GetComponent<Health>();
    }

    private void Start()
    {
        m_Health.HealthIsEmptyEvent += delegate
        {
            OnDamageableDeath?.Invoke();
        };
    }

    public void GetDamage(int damage)
    {
        m_Health.SetHealth(damage);
        DamageReceived?.Invoke();
    }

    private void ShowImpact(Vector3 position)
    {
        if (!m_ImpactParticle) return;
        m_ImpactParticle.transform.position = new Vector3(transform.position.x, position.y, transform.position.z);
        m_ImpactParticle?.Play();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag($"Projectile")) return;
        
        Bullet bullet = other.GetComponent<Bullet>();
        if(m_DamagerType is DamagerType.Enemy && bullet.IsFromEnemy()) return;

        GetDamage(-bullet.Damage);
        ShowImpact(other.ClosestPoint(transform.position));
        //Destroy(other.gameObject);
        other.gameObject.SetActive(false);
    }
}

public enum DamagerType
{
    Player,
    Enemy,
    Props
}


public interface IDamageable
{
    void GetDamage(int value);
}
