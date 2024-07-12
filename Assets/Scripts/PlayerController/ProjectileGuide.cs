using System;
using ProjectC.Armory;
using ProjectC.Other;
using UnityEngine;

namespace PlayerController
{
    public class ProjectileGuide : MonoBehaviour
    {
        private Vector3 _localScale;
        [SerializeField] private Transform m_GuideTransform;
        private bool _willShowGuide;

        private void Awake()
        {
            _localScale = new Vector3(10f, 20f, 10f);
        }

        private void OnEnable()
        {
            AttackController.WeaponEquippedEvent += OnWeaponEquippedEvent;
        }
        

        private void OnDisable()
        {
            AttackController.WeaponEquippedEvent -= OnWeaponEquippedEvent;
        }

        private void OnWeaponEquippedEvent(Weapon weapon)
        {
            _willShowGuide = weapon is Throwable;
            m_GuideTransform.gameObject.SetActive(_willShowGuide);
        }

        private void Update()
        {
            UpdateGuideLength();
        }

        private void UpdateGuideLength()
        {
            if (InputController.AimDirection.magnitude < 0.2f)
            {
                m_GuideTransform.gameObject.SetActive(false);
                return;
            }
            m_GuideTransform.gameObject.SetActive(_willShowGuide && InputController.AimDirection.magnitude > 0.2f);
            _localScale.x = 5f + InputController.AimDirection.magnitude * 40f;
            m_GuideTransform.localScale = _localScale;
        }
    }
}