using System;
using Casinoventure.UI;
using DG.Tweening;
using Other;
using ProjectC.Armory;
using ProjectC.Armory.Interfaces;
using ProjectC.Other;
using ProjectC.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using ZombieRoad.UI;

namespace PlayerController
{
    public class AttackController : MonoBehaviour
    {
        private static AttackController _instance;
        private InventoryController _inventoryController;
        [ShowInInspector, ReadOnly] private WeaponType _currentWeaponType;
        private AnimationHandler _animationHandler;
        private float _timer;
        [SerializeField] private float m_CurrentFireRate = 0.5f;
        [SerializeField, InlineEditor] private WeaponController m_GunController;
        [SerializeField, InlineEditor] private WeaponController m_ThrowableController;
        [SerializeField] private Weapon m_ActiveWeapon;
        [SerializeField] private int m_WeaponIndexToOpenWith = 0;
        [SerializeField] private float m_CurrentRange = 10f;
        [SerializeField] private float m_CurrentReloadDelay = 2f;
        [SerializeField] private Bullet m_Bullet;
        [SerializeField] private TimerUI m_TimerUI;
        
        [SerializeField] private Transform m_LaunchDirection;
        private Weapon _previousWeapon;
        private bool _throwCooldown;
        public int CurrentDamage => m_ActiveWeapon.Data.Damage;

        public static event Action<Weapon> WeaponEquippedEvent;
        public static event Action<Weapon> WeaponUpdateEvent;
        public static event Action<int, int> WeaponClipCountChanged;
        public static event Action<bool, Weapon> TriggeredWithWeaponEvent;
        public Bullet Bullet => m_Bullet;


        private int _nadeIndex;


        // private bool _isReloading;
        [ShowInInspector, ReadOnly] private int _currentClipCount;
        [ShowInInspector, ReadOnly] private AttackState _currentAttackState;
        [SerializeField] private float m_EquipTime = 1.5f;
        [SerializeField] private int m_CarryCapacity = 6;
        public int CurrentClipCount => _currentClipCount;

        public WeaponController GunController => m_GunController;

        public WeaponController GrenadeController => m_ThrowableController;

        public AttackState CurrentAttackState => _currentAttackState;

        public WeaponType CurrentWeaponType => _currentWeaponType;

        public Weapon ActiveWeapon => m_ActiveWeapon;

        private bool _shootCoolDown;
        [ShowInInspector, ReadOnly] private int _weaponIndexToSwapWith;
        private Pickable _currentPickable;
        [SerializeField] private bool m_UnlimitedAmmo;
        [SerializeField] private Transform m_MsgPos;
        public static event Action ThrowEndedEvent;
        private bool _isChargingThrow;
        private float _powerModifier;
        [SerializeField] private float m_MinPower = 5f;
        [SerializeField] private float m_MaxPower = 10f;

        private void Awake()
        {
            _instance = this;
            _isChargingThrow = false;
            _powerModifier = 0;
            _inventoryController = GetComponent<InventoryController>();
            _animationHandler = GetComponent<AnimationHandler>();
        }
        

        private void Start()
        {
            GetWeapon(m_WeaponIndexToOpenWith, WeaponType.Gun, true);
        }

        private void Update()
        {
            // if (InputController.AimDirection.magnitude > 0.9f) HandleShooting();
            if (Input.GetMouseButton(0)) HandleShooting();
            if (Input.GetKeyDown(KeyCode.R)) Reload();
            if (Input.GetKeyDown(KeyCode.Q)) _inventoryController.CycleWeapon(WeaponType.Throwable);
            if (Input.GetKeyDown(KeyCode.E)) _inventoryController.CycleWeapon(WeaponType.Gun);
            CalculateThrowingPower();
            if (Input.GetKeyUp(KeyCode.G)) HandleThrowing(_powerModifier);
        }

        private void CalculateThrowingPower()
        {
            if (Input.GetKeyDown(KeyCode.G)) _isChargingThrow = true;
            if(!_isChargingThrow) return;
            _powerModifier += Time.deltaTime;
            if (_powerModifier >= m_MaxPower)
            {
                _isChargingThrow = false;
            }
        }

        [Button]
        private void GetWeapon(int index, WeaponType weaponType, bool loadMax = false)
        {
            _inventoryController.RegisterWeaponToSlot(index, weaponType);
            EquipWeapon(index, weaponType, loadMax);
        }
        

        public void Reload()
        {
            if(_currentAttackState is AttackState.Reloading or AttackState.Equipping) return;
            if(m_ActiveWeapon is not IShooting shooter) return;
            if(shooter.RemainingClipSize <= 0 || shooter.CurrentClipCount >= m_ActiveWeapon.Data.MagazineSize) return;
            _currentAttackState = AttackState.Reloading;
            m_TimerUI.StartRingTimer(m_ActiveWeapon.Data.ReloadTime);
            _animationHandler.ResetAllWeaponRigs();
            _animationHandler.PlayReloadAnim();

            DOVirtual.DelayedCall(m_ActiveWeapon.Data.ReloadTime - 1f, () =>
                {
                    EquipWeapon(m_ActiveWeapon.Data.Index, WeaponType.Gun);
                    shooter.RefillAmmo();
                    _currentClipCount = shooter.CurrentClipCount;
                    WeaponClipCountChanged?.Invoke(_currentClipCount, shooter.RemainingClipSize);
                })
                .OnComplete(() =>
                DOVirtual
                    .DelayedCall(1f, () => 
                        _currentAttackState = AttackState.Idle));
        }

        private void SetWeapon(int index, WeaponType weaponType)
        {
            if (_previousWeapon) _previousWeapon.SetEquipState(false);
            var weaponController = weaponType is WeaponType.Gun ? m_GunController : m_ThrowableController;
            weaponController.ActivateWeapon(index, out m_ActiveWeapon);
            m_ActiveWeapon.SetEquipState(true);
            _previousWeapon = m_ActiveWeapon;
            SetBulletData(m_ActiveWeapon);
        }

        private void SetBulletData(Weapon weapon)
        {
            m_CurrentFireRate = weapon.Data.FireRate;
        }


        private void  HandleShooting()
        {
            if (m_ActiveWeapon is not IShooting shooter) return;
            if(_currentAttackState is AttackState.Reloading or AttackState.Equipping) return;
            if (shooter.CurrentClipCount <= 0 && shooter.RemainingClipSize <= 0)
            {
                GlobalSlideUpMessage.ShowMessage("No Ammo!",  m_MsgPos.transform.position);
                return;
            }
            _timer += Time.deltaTime;
            if (shooter.CurrentClipCount <= 0)
            {
                Reload();
                return;
            }
            
            if (_shootCoolDown)
            {
                if (_timer > m_CurrentFireRate)
                {
                    _timer = 0;
                    _shootCoolDown = false;
                }
                return;
            }
            Shoot();
            return;

            void Shoot()
            {
                _shootCoolDown = true;
                _currentAttackState = AttackState.Shooting;
                shooter.Shoot(transform.forward, 1f, m_ActiveWeapon.Data.Range);
                SimulateRecoil();
                _animationHandler.PlayKnockBack();
                if(!m_UnlimitedAmmo) shooter.UpdateClipCount();
                _currentClipCount = shooter.CurrentClipCount;
                WeaponClipCountChanged?.Invoke(_currentClipCount, shooter.RemainingClipSize);
            }
        }

        private void SimulateRecoil()
        {
            Gun gun = (Gun)m_ActiveWeapon;
            m_ActiveWeapon.transform.DOKill();
            gun.transform.localPosition = new Vector3(gun.transform.localPosition.x, gun.InitialY, gun.transform.localPosition.z);
            m_ActiveWeapon.transform.DOLocalMoveY(gun.InitialY - 0.05f, 0f)
                .OnComplete(() => gun.transform.DOLocalMoveY(gun.InitialY, 0.3f));

        }

        private void HandleThrowing(float power)
        {
            if (m_ActiveWeapon is not IThrowable throwable) return;
            if(_throwCooldown) return;
            _throwCooldown = true;
            power = Mathf.Clamp(power, m_MinPower, m_MaxPower);
            throwable.Throw(m_LaunchDirection.rotation, power);
            _powerModifier = 0;
            _currentClipCount = throwable.UpdateClip();
            WeaponClipCountChanged?.Invoke(_currentClipCount, -1);
            _animationHandler.PlayThrowAnim();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                
                if(_currentClipCount > 0) EquipWeapon(m_ActiveWeapon.Data.Index, WeaponType.Throwable);
                else ThrowEndedEvent?.Invoke();
            });
            DOVirtual.DelayedCall(1f, () => _throwCooldown = false);
        }

        public void AddClipToIndex(int index)
        {
            IThrowable throwable = m_ThrowableController.GetWeaponByIndex(index) as IThrowable;
            _inventoryController.RegisterWeaponToSlot(index, WeaponType.Throwable);
            WeaponUpdateEvent?.Invoke(m_ThrowableController.GetWeaponByIndex(index));
            WeaponClipCountChanged?.Invoke(throwable.CurrentClipCount, -1);
        }
        
        public void EquipWeapon(int index, WeaponType weaponType, bool refill = false)
        {
            _shootCoolDown = false;
            _currentAttackState = AttackState.Equipping;
            _currentWeaponType = weaponType;
            SetWeapon(index, _currentWeaponType);
            if (refill) m_ActiveWeapon.Refill();
            CameraController.SwitchCamera(m_ActiveWeapon.Data.Range + 5);
            HandleEquipAndClipData();
            WeaponUpdateEvent?.Invoke(m_ActiveWeapon);
            WeaponEquippedEvent?.Invoke(ActiveWeapon);
            DOVirtual.DelayedCall(m_ActiveWeapon.Data.EquipTime, () =>
            {
                _currentAttackState = AttackState.Idle;
            });

            void HandleEquipAndClipData()
            {
                if (_currentWeaponType is WeaponType.Gun)
                {
                    _animationHandler.PlayGunEquipAnim((int)m_ActiveWeapon.Data.HoldingStyle);
                    _currentClipCount = ((IShooting)m_ActiveWeapon).CurrentClipCount;
                    WeaponClipCountChanged?.Invoke(_currentClipCount, ((IShooting)m_ActiveWeapon).RemainingClipSize);
                }
                else
                {
                    _animationHandler.PlayThrowableEquipAnim();
                    _currentClipCount = ((IThrowable)m_ActiveWeapon).CurrentClipCount;
                    WeaponClipCountChanged?.Invoke(_currentClipCount, -1);
                }
            }
        }


        public void OnTriggeredWithWeapon(Pickable pickable, int index, bool state)
        {
            _currentPickable = pickable; 
            _weaponIndexToSwapWith = index;
            TriggeredWithWeaponEvent?.Invoke(state, GetWeaponRef(_weaponIndexToSwapWith, WeaponType.Gun));
        }

        public void SwapWeapon()
        {
            _currentPickable.Release();
            GetWeapon(_weaponIndexToSwapWith, WeaponType.Gun);
            TriggeredWithWeaponEvent?.Invoke(false, null);
        }

        public Weapon GetWeaponRef(int weaponIndex, WeaponType weaponType)
        {
            WeaponController weaponController = weaponType is WeaponType.Gun ? m_GunController : m_ThrowableController;
            return weaponController.GetWeaponByIndex(weaponIndex);
        }
    }
    public enum WeaponType
    {
        Gun,
        Throwable
    }

    public enum AttackState
    {
        Idle,
        Shooting,
        Equipping,
        Reloading
    }
}