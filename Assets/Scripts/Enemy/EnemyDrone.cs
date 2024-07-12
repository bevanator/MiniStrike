using System.Collections.Generic;
using Enemy;
using Other;
using PlayerController;
using Pool;
using ProjectC.Armory;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectC.Enemy
{
    public class EnemyDrone : EnemyBase
    {
        [SerializeField] private float m_MoveSpeed = 1f;
        private List<Vector3> _wayPoints = new();
        private int _currentIndex;

        private Health _health;
        private Shootable _shootable;
        
        // private static readonly int Death = Animator.StringToHash("Death");
        // private static readonly int Damage = Animator.StringToHash("Damage");

        [SerializeField] private ParticleSystem m_DeathParticle;
        [SerializeField] private ParticleSystem m_FireParticle;
        
        [SerializeField] private bool m_Looping = true;
        [SerializeField] private int m_NumberOfPoints = 5;
        [SerializeField] private Animator m_Animator;
        [ShowInInspector, ReadOnly] private EnemyState _currentState;
        [SerializeField] private Bullet m_BulletPrefab;
        private float _time;
        [SerializeField] private float m_ShootingDelay = 3f;
        [SerializeField] private Transform m_ShootingPoint;
        [SerializeField] private float m_ShootingDistance = 10f;
        private float _checkingTime;
        [SerializeField] private float m_CheckingDelay = 1f;
        [SerializeField] private int m_Damage;

        private Rigidbody _rigidbody;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _shootable = GetComponent<Shootable>();
            _health = GetComponent<Health>();
            _currentState = EnemyState.Approaching;
            _checkingTime = m_CheckingDelay;
        }

        private void OnEnable()
        {
            _health.HealthIsEmptyEvent += HealthIsEmptyEvent;
            _shootable.DamageReceived += OnDamageReceivedEvent;
        }

        private void OnDisable()
        {
            _health.HealthIsEmptyEvent -= HealthIsEmptyEvent;
            _shootable.DamageReceived -= OnDamageReceivedEvent;
        }

        public void Init(float speed, float fireRate, int damage)
        {
            m_MoveSpeed = speed;
            m_ShootingDelay = fireRate;
            m_Damage = damage;
        }

        private void OnDamageReceivedEvent()
        {
            // m_Animator.SetTrigger(Damage);
            if((float)_health.CurrentHealth/_health.MaxHealth > 0.7f) return;
            if(_currentState is EnemyState.Idle) _currentState = EnemyState.Approaching;
        }

        private void HealthIsEmptyEvent()
        {
            // m_Animator.SetTrigger(Death);
            _rigidbody.isKinematic = false;
            m_DeathParticle.Play();
            _health.HealthIsEmptyEvent -= HealthIsEmptyEvent;
        }

        

        private void Update()
        {
            transform.LookAt(Player.Instance.transform.position + Vector3.up);
            CheckForPlayersPosition();
            switch (_currentState)
            {
                // case EnemyState.Roaming:
                //     MoveBetweenTargets();
                //     break;
                case EnemyState.Approaching:
                    MoveToTarget(Player.Instance.transform.position + Vector3.up);
                    break;
                case EnemyState.Attacking:
                    Attack();
                    break;
            }
        }

        private void CheckForPlayersPosition()
        {
            _checkingTime -= Time.deltaTime;
            if (_checkingTime <= 0)
            {
                float distance = Vector3.Distance(transform.position, Player.Instance.transform.position + Vector3.up);
                if (distance >= m_ShootingDistance)
                {
                    _currentState = EnemyState.Approaching;
                }
                _checkingTime = m_CheckingDelay;
            }
            
        }

        private void Attack()
        {
            _time -= Time.deltaTime;
            if (_time <= 0)
            { 
                ShootFromPoint(m_ShootingPoint);
                _time = m_ShootingDelay;
            }
        }
        
        private void ShootFromPoint(Transform source)
        {
            m_FireParticle.Play();
            Bullet bullet = Instantiate(m_BulletPrefab, source.position, transform.rotation);
            // Bullet bullet = PoolManager.Instance.BulletPool.Get(m_BulletPrefab);
            bullet.transform.SetPositionAndRotation(source.position, transform.rotation);
            bullet.Init(100, m_Damage);
        }

        private void MoveToTarget(Vector3 target)
        {
            transform.LookAt(target);
            if (Vector3.Distance(transform.position, target) <= m_ShootingDistance)
            {
                _currentState = EnemyState.Attacking;
                return;
            }
            float step = m_MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            
        }

        private enum EnemyState
        {
            Idle,
            Approaching,
            Attacking,
            Dead
        }
        
        #region UnUsed

        private void Start()
        {
            // InitWayPoints();
            // _target = Player.Instance.transform.position + Vector3.up;
        }

        private void InitWayPoints()
        {
            for (int i = 0; i < m_NumberOfPoints; i++)
            {
                Vector2 point = Random.insideUnitCircle * 5;
                _wayPoints.Add(new Vector3(point.x, 1.5f, point.y));
            }

            transform.LookAt(_wayPoints[0]);
        }
        
        private void MoveBetweenTargets()
        {
            if (!m_Looping && _currentIndex == m_NumberOfPoints - 1) return;
            float step = m_MoveSpeed * Time.deltaTime;
            Vector3 currentTarget = _wayPoints[_currentIndex];
            if (Vector3.Distance(transform.position, currentTarget) <= 0.1f)
            {
                if (_currentIndex == m_NumberOfPoints - 1)
                {
                    _currentIndex = m_Looping ? 0 : m_NumberOfPoints - 1;
                    transform.LookAt(currentTarget);
                    return;
                }
                currentTarget = _wayPoints[++_currentIndex];
                transform.LookAt(currentTarget);
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);
        }

        public void DestroySelf()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
    
}