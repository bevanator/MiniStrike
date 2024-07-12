using DG.Tweening;
using EGF.UI;
using PlayerController;
using ProjectC.Armory;
using ProjectC.Data;
using ProjectC.Other;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WeaponUI : UIPanelBase
    {
        private AttackController _attackController;
        private InventoryController _inventoryController;
        [SerializeField] private TextMeshProUGUI m_GunClipText;
        [SerializeField] private TextMeshProUGUI m_GunClipReserveText;
        [SerializeField] private TextMeshProUGUI m_GrenadeCountText;
        
        
        [SerializeField] private Image m_GunImage;
        [SerializeField] private Image m_GrenadeImage;
        [SerializeField] private Button m_SwitchButton;
        [SerializeField] private Button m_GrenadeSwitchButton;
        [SerializeField] private Button m_ReloadButton;
        [SerializeField] private Button m_SwapButton;
        [SerializeField] private RectTransform m_SwapRect;
        [SerializeField] private Image m_SwapWeaponImage;
        

        private void Awake()
        {
            _inventoryController = FindFirstObjectByType<InventoryController>();
            _attackController = FindFirstObjectByType<AttackController>();
        }

        private void OnEnable()
        {
            AttackController.WeaponUpdateEvent += OnWeaponUpdateEvent;
            AttackController.WeaponClipCountChanged += OnWeaponClipCountChanged;
            AttackController.TriggeredWithWeaponEvent += SetSwapActionDisplayState;
        }

        private void OnDisable()
        {
            AttackController.WeaponUpdateEvent -= OnWeaponUpdateEvent;
            AttackController.WeaponClipCountChanged -= OnWeaponClipCountChanged;
            AttackController.TriggeredWithWeaponEvent -= SetSwapActionDisplayState;
        }

        private void OnWeaponClipCountChanged(int count, int reserve)
        {
            if (reserve is -1)
            {
                m_GrenadeCountText.text = count.ToString();
                return;
            }
            m_GunClipText.text = count.ToString() + "/";
            m_GunClipReserveText.text = reserve.ToString();
        }

        private void OnWeaponUpdateEvent(Weapon weapon)
        {
            Image weaponImage;
            WeaponData data = weapon.Data;
            weaponImage = data.WeaponType is WeaponType.Gun ? m_GunImage : m_GrenadeImage;
            weaponImage.sprite = data.Icon;
            weaponImage.rectTransform.DOKill();
            weaponImage.rectTransform.localScale = Vector3.one;
            RectPopIn(weaponImage.rectTransform);
        }

        private void Start()
        {
            m_SwitchButton.onClick.AddListener(() =>
            {
                _inventoryController.CycleWeapon(WeaponType.Gun);
            });
            
            m_GrenadeSwitchButton.onClick.AddListener(() =>
            {
                _inventoryController.CycleWeapon(WeaponType.Throwable);
            });
            
            m_ReloadButton.onClick.AddListener(() =>
            {
                _attackController.Reload();
            });
            
            m_SwapButton.onClick.AddListener(() =>
            {
                _attackController.SwapWeapon();
            });
        }

        private void SetSwapActionDisplayState(bool state, Weapon weapon)
        {
            m_SwapRect.gameObject.SetActive(state);
            if(state) m_SwapWeaponImage.sprite = weapon.Data.Icon;
        }
    }
}