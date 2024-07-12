using System.Collections.Generic;
using System.Linq;
using PlayerController;
using ProjectC.Armory;
using UnityEngine;

namespace ProjectC.Data
{
    [CreateAssetMenu(fileName = "WeaponDataList", menuName = "Weapon Data List", order = 0)]
    public class WeaponDataList : ScriptableObject
    {
        [SerializeField] private List<WeaponData> m_WeaponDataList = new List<WeaponData>();
        
        public WeaponData GetWeaponDataByIndex(int index) => m_WeaponDataList[index];
    }
    [System.Serializable]
    public class WeaponData
    {
        public string Name;
        public Sprite Icon;
        public int Damage;
        public float FireRate;
        public float Range;
        public int Multiplier;
        public int TotalClipSize;
        public int MagazineSize;
        public WeaponType WeaponType;
        public WeaponHoldStyle HoldingStyle = WeaponHoldStyle.Pistol;
        public GunType GunRoleType = GunType.Primary;
        public int Index = 0;
        public float ReloadTime = 2f;

        public float EquipTime = 1.5f;
        public Bullet Bullet;
    }

    public enum WeaponHoldStyle
    {
        Pistol,
        Rifle
    }
    
    public enum GunType
    {
        Secondary,
        Primary
    }
}