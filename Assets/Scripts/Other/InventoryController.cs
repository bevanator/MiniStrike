using System;
using System.Collections.Generic;
using PlayerController;
using ProjectC.Armory;
using ProjectC.Armory.Interfaces;
using ProjectC.Data;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace ProjectC.Other
{
    public class InventoryController : MonoBehaviour
    {

        [SerializeField] private SlotSystem m_GunSlotSystem;
        [SerializeField] private SlotSystem m_ThrowableSlotSystem;
        
        private AttackController _attackController;
        private int _equippedGunSlotIndex;
        private int _equippedThrowableSlotIndex;

        [SerializeField, Range(0,7)] private int m_NoOfGunsToRegister = 1;
        [SerializeField, Range(0,2)] private int m_NoOfGrenadesToRegister = 1;


        [SerializeField] private List<Pickable> m_CarrierList = new();
        [SerializeField] private WeaponDataList m_WeaponDataList;

        private WeaponType _currentWeaponType;


        private void Awake()
        {
            _attackController = GetComponent<AttackController>();
            for (int i = 0; i < m_NoOfGunsToRegister; i++) RegisterWeaponToSlot(i, WeaponType.Gun);
            for (int i = 0; i < m_NoOfGrenadesToRegister; i++) RegisterWeaponToSlot(i, WeaponType.Throwable);
            SetUpWeaponData();
        }


        private void OnEnable()
        {
            AttackController.ThrowEndedEvent += OnThrowEndedEvent;
        }
        private void OnDisable()
        {
            AttackController.ThrowEndedEvent -= OnThrowEndedEvent;
        }
        
        private void SetUpWeaponData()
        {
            for (int i = 0; i < m_CarrierList.Count; i++)
            {
                m_CarrierList[i].Init(i);
            }
        }

        private void OnThrowEndedEvent()
        {
            if(m_ThrowableSlotSystem.GetNextLoadedThrowable() is null) 
                CycleWeapon(WeaponType.Gun);
            else 
                CycleWeapon(WeaponType.Throwable);
        }

        private void RegisterToSlot(Weapon weapon)
        {
            if (weapon is IShooting shooter) RegisterGun();
            else RegisterThrowable();
            
            void RegisterThrowable()
            {
                if (m_ThrowableSlotSystem.Slots.Contains(weapon))
                {
                    ((IThrowable)weapon).AddClip();
                    return;
                }
                if (m_ThrowableSlotSystem.Slots.Count >= m_ThrowableSlotSystem.MaxSlotCount)
                {
                    return;
                }
                m_ThrowableSlotSystem.Slots.Add(weapon);
                m_ThrowableSlotSystem.UpdateEquippedSlot(weapon);

            }
            void RegisterGun()
            {
                if (m_GunSlotSystem.Slots.Contains(weapon))
                {
                    shooter.RefillAmmo(true);
                    return;
                }
                if (weapon.Data.GunRoleType is GunType.Secondary)
                {
                    if(!m_GunSlotSystem.Slots.IsNullOrEmpty()) m_GunSlotSystem.Slots[0] = weapon;
                    else m_GunSlotSystem.Slots.Add(weapon);
                }
                else
                {
                    if (m_GunSlotSystem.Slots.Count >= m_GunSlotSystem.MaxSlotCount)
                    {
                        int dropIndex = m_GunSlotSystem.Slots[1].Data.Index;
                        m_CarrierList[dropIndex].gameObject.SetActive(false);
                        // foreach (Pickable pickable in m_CarrierList) pickable.gameObject.SetActive(false);
                        m_CarrierList[dropIndex].Drop(transform.position);
                        m_GunSlotSystem.Slots[1] = weapon;
                        return;
                    }
                    m_GunSlotSystem.Slots.Add(weapon);
                    m_GunSlotSystem.UpdateEquippedSlot(weapon);
                }
            }
            
        }
        
        [Button]
        public void RegisterWeaponToSlot(int weaponIndex, WeaponType weaponType)
        {
            WeaponController weaponController = weaponType is WeaponType.Gun ? _attackController.GunController : _attackController.GrenadeController;
            Weapon weapon = weaponController.GetWeaponByIndex(weaponIndex);
            RegisterToSlot(weapon);
        }
        

        public void CycleWeapon(WeaponType weaponType)
        {
            SlotSystem appropriateSlotSystem = weaponType is WeaponType.Gun ? m_GunSlotSystem : m_ThrowableSlotSystem;
            CycleAppropriateWeapon(weaponType, appropriateSlotSystem);
        }

        public bool IsWeaponInInventory(Weapon weapon)
        {
            SlotSystem slotSystem = GetAppropriateSlot(weapon.Data.WeaponType);
            return slotSystem.IsInSlots(weapon);
        }

        private SlotSystem GetAppropriateSlot(WeaponType weaponType)
        {
            return weaponType is WeaponType.Gun ? m_GunSlotSystem : m_ThrowableSlotSystem;
        }

        private void CycleAppropriateWeapon(WeaponType weaponType, SlotSystem slotSystem)
        {
            if(slotSystem.Slots.IsNullOrEmpty()) return;
            if (weaponType is WeaponType.Throwable)
            {
                bool checkSelf = false;
                checkSelf = _currentWeaponType is not WeaponType.Throwable;
                if(m_ThrowableSlotSystem.GetNextLoadedThrowable(checkSelf) is null) return;
            }
            
            if(_attackController.CurrentAttackState is AttackState.Equipping) return;
            if (_attackController.CurrentWeaponType == weaponType)
            {            
                if (slotSystem.EquippedSlot >= slotSystem.Slots.Count-1) slotSystem.EquippedSlot = 0;
                else slotSystem.EquippedSlot++;
            }
            
            _attackController.EquipWeapon(slotSystem.GetCurrentActiveSlot().Data.Index, weaponType);
            _currentWeaponType = weaponType;
        }
        
        [System.Serializable]
        private class SlotSystem
        {
            [field:SerializeField, ReadOnly] public List<Weapon> Slots { get; set; }
            [field:SerializeField, ReadOnly] public int EquippedSlot { get; set; }
            [field:SerializeField] public int MaxSlotCount { get; set; }
            public bool IsInSlots(Weapon weapon) => Slots.Contains(weapon);

            public Weapon GetCurrentActiveSlot() => Slots[EquippedSlot];

            public Weapon GetNextLoadedThrowable(bool checkSelf = false)
            {
                foreach (var weapon in Slots)
                {
                    if (!checkSelf)
                    {
                        if(weapon == GetCurrentActiveSlot()) continue;
                    }
                    if (weapon is not IThrowable throwable) continue;
                    if (throwable.CurrentClipCount > 0) return weapon;
                }

                return null;
            }

            public void UpdateEquippedSlot(Weapon weapon)
            {
                EquippedSlot = Slots.IndexOf(weapon);
            }
        }

        public void OnEquipStateChanged(Weapon weapon, bool state)
        {
            if (weapon is IThrowable) return;
            if(m_GunSlotSystem.IsInSlots(weapon)) m_CarrierList[weapon.Data.Index].gameObject.SetActive(!state);
        }
    }
}