using DG.Tweening;
using Pool;
using ProjectC.Constants;
using UnityEngine;

namespace ProjectC.Armory
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        private bool _isActive;
        private TrailRenderer _trail;
        private Vector3 _spawnPos;
        private Rigidbody _rb;
        private float _startTime;
        private Vector3 _targetPos;
        private float _maxDistance = 500f;
        private DamagerType _damagerType;
        [SerializeField] private float m_Speed;
        [SerializeField] private float m_Range = 10f;
        [SerializeField] private int m_Damage = 20;
        [SerializeField] private float m_RangeMultiplier = 1.5f;
        [SerializeField] private ParticleSystem m_ImpactParticle;
        [SerializeField] private bool m_EnemyBullet;
        public int Damage => m_Damage;

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _rb = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate() 
        {
            if(!_isActive) return;
            float distCovered = (Time.time - _startTime) * m_Speed;
            float distance = Vector3.Distance(_spawnPos, transform.position);
            float fractionOfJourney = distCovered / _maxDistance;
            _rb.MovePosition(Vector3.Lerp(transform.position, _targetPos, fractionOfJourney));
            if (distance >= m_Range)
            {
                ReleaseBackToPool();
            }
        }


        private void Clear()
        {
            _isActive = false;
        }



        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer != GameLayers.SHOOTABLE) return;
            if (m_EnemyBullet && other.gameObject.CompareTag(GameTags.ENEMY)) return;
            _rb.velocity = Vector3.zero;
            _isActive = false;
            if(!other.gameObject.TryGetComponent(out IDamageable damageable)) m_ImpactParticle.Play();
            // ReleaseBackToPool();
            DOVirtual.DelayedCall(0.05f, () =>
            {
                ReleaseBackToPool();
            });

        }


        public void Init(float range = 10f, int damage = 20, DamagerType damagerType = DamagerType.Player)
        {
            m_Range = range;
            m_Damage = damage;
            _damagerType = damagerType;
            _spawnPos = transform.position;
            _startTime = Time.time;
            _targetPos = transform.position + _maxDistance * transform.forward;
            _isActive = true;
            gameObject.SetActive(true);
        }
        
        public IPoolable OriginalPoolPrefab { get; set; }
        public void ReleaseBackToPool()
        {
            Clear();
            _trail.Clear();
            transform.position = _spawnPos;
            PoolManager.Instance.BulletPool.Release(this);
            // gameObject.SetActive(false);
        }

        public bool IsFromEnemy()
        {
            return _damagerType is DamagerType.Enemy;
        }
    }
}