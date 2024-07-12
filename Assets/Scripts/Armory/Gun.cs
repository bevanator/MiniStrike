using PlayerController;
using Pool;
using ProjectC.Armory.Interfaces;
using ProjectC.Data;
using UnityEngine;

namespace ProjectC.Armory
{
    public class Gun : Weapon, IShooting
    {
        private AudioSource _audioSource;
        private AttackController _attackController;
        
        [SerializeField] private int m_CurrentClipCount = 30;
        public int CurrentClipCount => m_CurrentClipCount;
        
        [SerializeField] private int m_RemainingClipSize = 120;
        public int RemainingClipSize => m_RemainingClipSize;

        public float InitialY => _initialY;


        private float _initialY;

        [SerializeField] private ParticleSystem m_FireParticle;
        [SerializeField] private Transform m_FirePoint;
        [SerializeField] private AudioClip m_ShootAudioClip;
        [SerializeField] private bool m_InitialReload;
        [SerializeField] private Transform m_Geometry;
        
        public bool InitialReload => m_InitialReload;



        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _attackController = GetComponentInParent<AttackController>();
        }

        private void Start()
        {
            _audioSource.clip = m_ShootAudioClip;
            PoolManager.Instance.BulletPool.Initialize(Data.Bullet, 20);
        }

        public override void Init(WeaponData weaponData, int index)
        {
            base.Init(weaponData, index);
            m_RemainingClipSize = Data.TotalClipSize;
            m_CurrentClipCount = Data.MagazineSize;
            m_RemainingClipSize -= CurrentClipCount;
            _initialY = transform.localPosition.y;
        }

        public void Shoot(Vector3 forward, float damageMultiplier = 1f, float range = 50, DamagerType damagerType = DamagerType.Player)
        {
            m_FireParticle.Play();
            _audioSource.Play();
            // Bullet bullet = Instantiate(Data.Bullet, m_FirePoint.position, rotation);
            Bullet bullet = PoolManager.Instance.BulletPool.Get();
            bullet.gameObject.SetActive(false);
            // bullet.transform.SetPositionAndRotation(m_FirePoint.position, rotation);
            bullet.transform.forward = forward;
            bullet.transform.position = m_FirePoint.position;
            // bullet.Launch(range, Data.Damage);
            bullet.Init(range, (int)(damageMultiplier * Data.Damage), damagerType);
        }
        

        public int UpdateClipCount()
        {
            return --m_CurrentClipCount;
        }

        public void RefillAmmo(bool maxClip = false)
        {
            int capacity = Data.MagazineSize - m_CurrentClipCount;
            int amountToReduce = m_RemainingClipSize < Data.MagazineSize ? m_RemainingClipSize : capacity;
            m_RemainingClipSize = maxClip ? Data.TotalClipSize : m_RemainingClipSize - amountToReduce;
            m_CurrentClipCount += amountToReduce;
        }

        public override void Refill()
        {
            RefillAmmo();
        }
        
    }
}