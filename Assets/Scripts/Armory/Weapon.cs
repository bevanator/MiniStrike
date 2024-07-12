using Sirenix.OdinInspector;
using UnityEngine;
using ProjectC.Data;
using ProjectC.Other;

namespace ProjectC.Armory
{
    public class Weapon : MonoBehaviour
    {
        [ShowInInspector] private WeaponData _weaponData = new WeaponData();
        private bool _isEquipped;
        public WeaponData Data => _weaponData;
        public bool IsEquipped => _isEquipped;

        [SerializeField] protected InventoryController m_InventoryController;
        

        public virtual void Init(WeaponData weaponData, int index)
        {
            _weaponData = weaponData;
            _weaponData.Index = index;
        }

        public virtual void Refill()
        {
            
        }
        
        public void SetEquipState(bool state)
        {
            gameObject.SetActive(state);
            _isEquipped = state;
            m_InventoryController.OnEquipStateChanged(this, state);
        }
    }
}

