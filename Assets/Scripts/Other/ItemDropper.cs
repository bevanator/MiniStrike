using System.Collections.Generic;
using ProjectC.Other;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Other
{
    public class ItemDropper : MonoBehaviour
    {
        [SerializeField] private List<Pickable> m_Pickables;

        [Button]
        public void DropWeaponByIndex(int index)
        {
            m_Pickables[index].Drop(transform.position);
        }
    }
}