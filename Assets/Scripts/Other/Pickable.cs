using System;
using DG.Tweening;
using PlayerController;
using ProjectC.Armory;
using ProjectC.Constants;
using ProjectC.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectC.Other
{
    public class Pickable : MonoBehaviour
    {
        private AttackController _attackController;
        private Weapon _weapon;
        [SerializeField] private int m_Index;
        [SerializeField] private bool m_IsThrowable;
        [SerializeField] private bool m_DisableOnPickUp;
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private Collider m_Collider;

        private void Awake()
        {
            _attackController = FindFirstObjectByType<AttackController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.gameObject.CompareTag(GameTags.PLAYER)) return;
            if(!m_IsThrowable) _attackController.OnTriggeredWithWeapon(this, m_Index, true);
            else
            {
                _attackController.AddClipToIndex(m_Index);
                Release();
            }
        }
        
        private void OnTriggerExit(Collider other)              
        {
            if(!other.gameObject.CompareTag(GameTags.PLAYER)) return;
            if(!m_IsThrowable) _attackController.OnTriggeredWithWeapon(this, m_Index, false);
        }

        public void Init(int i)
        {
            m_Index = i;
            _attackController = FindFirstObjectByType<AttackController>();
            _weapon = _attackController.GunController.GetWeaponByIndex(i);
        }
        

        [Button]
        public void Drop(Vector3 position)
        {
            Pickable drop = Instantiate(this, position, transform.rotation, null);
            drop.Init(m_Index);
            drop.gameObject.SetActive(true);
            drop.m_Rigidbody.AddForce(2f * transform.forward + transform.up, ForceMode.Impulse);
            drop.m_Rigidbody.isKinematic = false;
            DOVirtual.DelayedCall(1f, () =>
            {
                enabled = true;
                drop.m_Collider.enabled = true;
                drop.m_Collider.isTrigger = true;
            });
        }

        public void Release()
        {
            if(!m_DisableOnPickUp) return;
            gameObject.SetActive(false);
        }
    }
}