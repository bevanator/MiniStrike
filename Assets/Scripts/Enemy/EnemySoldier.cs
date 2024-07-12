using DG.Tweening;
using EGF.Utilities;
using Enemy.Interfaces;
using Other;
using PlayerController;
using Pool;
using ProjectC;
using ProjectC.AI;
using ProjectC.Armory;
using ProjectC.Constants;
using ProjectC.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Enemy
{
    public class EnemySoldier : EnemyBase, IAttacker, IApproacher, IPositionChecker
    {
        private Health _health;
        private Shootable _shootable;
        private float _time;
        private float _checkingTime;

        [SerializeField] private float m_MoveSpeed = 1f;
        [SerializeField] private ParticleSystem m_DeathParticle;
        [SerializeField] private ParticleSystem m_FireParticle;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private RigBuilder m_RigBuilder;
        [ShowInInspector, ReadOnly] private EnemyState _currentState;
        [SerializeField] private Bullet m_BulletPrefab;
        [SerializeField] private float m_ShootingDistance = 10f;
        [SerializeField] private float m_CheckingDelay = 1f;

        private Rigidbody _rigidbody;
        private NavmeshHandler _navmeshHandler;


        [SerializeField] private WeaponController m_WeaponController;
        private Weapon _activeWeapon;
        private float _damageMultiplier = 0.2f;
        private float _fireRateFactor = 2f;
        [SerializeField] private bool m_Shoot = true;
        private CharacterController _characterController;
        [SerializeField] private ItemDropper m_ItemDropper;
        private int _weaponIndex;
        [SerializeField] private LayerMask m_LayerMask;
        private bool _isSearching;
        private Player _player;
        [SerializeField] private bool m_UseCustomInit;
        [SerializeField] private int m_WeaponIndex = 1;
        [SerializeField] private Transform m_UISpace;
        


        private void Awake()
        {
            _player = FindFirstObjectByType<Player>();
            _characterController = GetComponent<CharacterController>();
            _navmeshHandler = GetComponent<NavmeshHandler>();
            _rigidbody = GetComponent<Rigidbody>();
            _shootable = GetComponent<Shootable>();
            _health = GetComponent<Health>();
            _currentState = EnemyState.Approaching;
            m_Animator.SetFloat(AnimHash.BlendY, 1f);
        }

        private void Start()
        {
            if(m_UseCustomInit) Init(m_WeaponIndex, 3f, 0.2f);
        }

        public void Init(int weaponIndex, float speed = 3f, float damageMultiplier = 0.2f, float fireRateFactor = 2f)
        {
            _weaponIndex = weaponIndex;
            _damageMultiplier = damageMultiplier;
            _fireRateFactor = fireRateFactor;
            m_MoveSpeed = speed;
            m_WeaponController.ActivateWeapon(weaponIndex, out _activeWeapon);
            _navmeshHandler.SetStoppingDistance(m_ShootingDistance);
            _navmeshHandler.SetSpeed(m_MoveSpeed);
            _currentState = IsPlayerInWorld() ? EnemyState.Approaching : EnemyState.Idle;
        }

        private bool IsPlayerInWorld()=>_player;

        private void GoIdle()
        {
            _currentState = EnemyState.Idle;
            m_Shoot = false;
            m_Animator.SetBool(AnimHash.Running, false);
        }
        
        private void OnEnable()
        {
            _health.HealthIsEmptyEvent += OnHealthEmptyEvent;
            _health.HealthModifiedEvent += OnDamageReceived;
            _navmeshHandler.DestinationReachedEvent += OnDestinationReachedEvent;
        }

        private void OnDisable()
        {
            _health.HealthIsEmptyEvent -= OnHealthEmptyEvent;
            _health.HealthModifiedEvent -= OnDamageReceived;
            _navmeshHandler.DestinationReachedEvent -= OnDestinationReachedEvent;
        }

        private void OnDamageReceived(float value)
        {
            PopNumbersUI popNumber = PoolManager.Instance.PopNumberPool.Get();
            popNumber.transform.parent = m_UISpace;
            popNumber.Pop("+" + Mathf.Abs((int)value));
        }

        private void OnHealthEmptyEvent()
        {
            m_ItemDropper.DropWeaponByIndex(_weaponIndex);
            DropRandomThrowable();
            _activeWeapon.gameObject.SetActive(false);
            Die();
            return;
            void DropRandomThrowable()
            {
                bool willDropThrowable = Helper.BinaryWeightedRandom(true, false, 0.5, 0.5);
                if (willDropThrowable)
                {
                    int randomThrowable = 7 + Helper.BinaryWeightedRandom(0, 1, 0.7, 0.3);
                    m_ItemDropper.DropWeaponByIndex(randomThrowable);
                }
            }
        }
        
        
        
        [Button]
        protected override void Die()
        {
            base.Die();
            _navmeshHandler.Stop();
            m_Shoot = false;
            _currentState = EnemyState.Dead;
            m_RigBuilder.enabled = false;
            _characterController.enabled = false;
            _shootable.enabled = false;
            m_Animator.SetLayerWeight(1, 0f);
            m_Animator.SetLayerWeight(2, 0f);
            m_Animator.SetTrigger(AnimHash.Death);
            DOVirtual.DelayedCall(2f, () =>
            {
                gameObject.SetActive(false);
                // Destroy(gameObject);
            });     
        }

        private void Update()
        {
            switch (_currentState)
            {
                case EnemyState.Idle:
                    GoIdle();
                    break;
                case EnemyState.Approaching:
                    FindTarget();
                    break;
                case EnemyState.Attacking:
                    Attack();
                    break;
                case EnemyState.Dead:
                    return;
            }
            CheckForPlayersPosition();
        }

        private void FindTarget()
        {
            if(!IsPlayerInWorld()) return;
            if(_isSearching) return;
            _isSearching = true;
            MoveToTarget(_player.transform.position);
        }

        private void OnDestinationReachedEvent()
        {
            if(!IsPlayerInWorld()) return;
            transform.DOLookAt(_player.transform.position, 0.1f).OnComplete(() =>
            {
                if (!CanSeePlayer(m_ShootingDistance, m_LayerMask))
                {
                    _navmeshHandler.SetStoppingDistance(m_ShootingDistance * 0.5f);
                    MoveToTarget(_player.transform.position);
                    return;
                }
                _currentState = EnemyState.Attacking;
                m_Animator.SetBool(AnimHash.Running, false);
                Attack();
            });

        }

        public void Attack()
        {
            if(!IsPlayerInWorld()) return;
            if(!m_Shoot) return;
            _isSearching = false;
            _time -= Time.deltaTime;
            if (_time <= 0)
            { 
                if (!CanSeePlayer(m_ShootingDistance, m_LayerMask))
                {
                    _currentState = EnemyState.Approaching;
                    MoveToTarget(_player.transform.position);
                    return;
                }

                transform.DOLookAt(_player.transform.position, 0.1f);
                ((Gun)_activeWeapon).Shoot(transform.forward, _damageMultiplier, damagerType:DamagerType.Enemy);
                _time = _activeWeapon.Data.FireRate * _fireRateFactor;
            }
        }

        public void MoveToTarget(Vector3 target)
        {
            if(!IsPlayerInWorld()) return;
            _navmeshHandler.GoToDestination(target);
            m_Animator.SetBool(AnimHash.Running, true);
        }

        public void CheckForPlayersPosition()
        {
            if(!IsPlayerInWorld()) return;
            _checkingTime -= Time.deltaTime;
            if (_checkingTime <= 0)
            {
                float distance = Vector3.Distance(transform.position, _player.transform.position + Vector3.up);
                _currentState = distance <= m_ShootingDistance ? EnemyState.Attacking : EnemyState.Approaching;
                _checkingTime = m_CheckingDelay;
            }
        }

        private enum EnemyState
        {
            Idle,
            Approaching,
            Attacking,
            Dead
        }
        
    }
    
}