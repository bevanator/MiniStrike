using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ProjectC;
using ProjectC.Armory;
using ProjectC.Data;
using Sirenix.OdinInspector;

namespace PlayerController
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Weapon m_ActiveWeapon;
        [SerializeField] private List<Weapon> m_Weapons = new List<Weapon>();
        [SerializeField] private bool m_GetAllWeaponAtAwake;
        [SerializeField] private WeaponDataList m_WeaponDataList;
        public Weapon ActiveWeapon => m_ActiveWeapon;

        private void Awake()
        {
            if (m_GetAllWeaponAtAwake) GetAllWeapons();
            SetUpWeaponData();
        }

        [Button]
        private void SetUpWeaponData()
        {
            for (int i = 0; i < m_Weapons.Count; i++)
            {
                m_Weapons[i].Init(m_WeaponDataList.GetWeaponDataByIndex(i), i);
            }
        }
        
        public void ActivateWeapon(int index, out Weapon activeWeapon)
        {
            foreach (Weapon weapon in m_Weapons) weapon.gameObject.SetActive(false);
            m_ActiveWeapon = m_Weapons[index];
            m_ActiveWeapon.gameObject.SetActive(true);
            activeWeapon = m_ActiveWeapon;
        }
        

        [Button]
        private void GetAllWeapons()
        {
            m_Weapons = GetComponentsInChildren<Weapon>(true).ToList();
        }

        public Weapon GetWeaponByIndex(int gunIndex)
        {
            return m_Weapons[gunIndex];
        }
    }
}

