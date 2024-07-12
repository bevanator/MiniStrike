using System;
using EGF.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieRoad.UI
{
    public class AlertWindow : UIPanelBase
    {
        private static AlertWindow Instance;

        [SerializeField] private Button m_CloseButton;

        [SerializeField] private TextMeshProUGUI m_MessageText;
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private TextMeshProUGUI m_CloseButtonText;
        
        [SerializeField] private RectTransform m_TitleBar;
        
        private Action _action;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            
                m_CloseButton.onClick.AddListener(delegate
                {
                    // AudioManager.ButtonClick();
                    Hide();
                });
                
                // base.Awake();    
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }


        [Button]
        public static void ShowMessage(string msg = "!", string title = "", string exitMsg = "Okay", Action onCompleteAction = null)
        {
            Instance.m_TitleText.text = title;
            if(title=="") Instance.m_TitleBar.gameObject.SetActive(false);
            Instance.m_CloseButtonText.text = exitMsg;
            Instance.m_MessageText.text = msg;
            Instance._action = onCompleteAction;
            Instance.Show();
        }


        protected override void OnHideCompleted()
        {
            base.OnHideCompleted();
            _action?.Invoke();
        }
    }
}