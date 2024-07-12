using ProjectC.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectC.Armory
{
    public class Throwable : Weapon, IThrowable
    {
        [SerializeField] private ThrowableDamager m_DamagerPrefab;
        
        [SerializeField] private int m_CurrentClipCount = 30;
        [SerializeField] private Transform m_Geometry;
        public int CurrentClipCount => m_CurrentClipCount;
        

        public override void Init(WeaponData weaponData, int index)
        {
            base.Init(weaponData, index);
            m_CurrentClipCount = Data.MagazineSize;
        }

        public void Throw(Quaternion rotation, float power)
        {
            var damager = Instantiate(m_DamagerPrefab, transform.position + 0.5f * (transform.forward + transform.up), Quaternion.identity, null);
            damager.Propel(rotation, power);
        }

        public int UpdateClip() => --m_CurrentClipCount;

        private void RefillGrenades() => m_CurrentClipCount = Data.TotalClipSize;

        public override void Refill()
        {
            RefillGrenades();
        }

        public void AddClip()
        {
            m_CurrentClipCount++;
        }
    }
}