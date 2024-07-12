using DG.Tweening;
using EGF.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TutorialUI : UIPanelBase
    {
        [SerializeField] private Button m_ContinueButton;
        [SerializeField] private RectTransform m_TutorialPanel;

        private void Awake()
        {
            Show();
            RectPopIn(m_TutorialPanel);
            DOVirtual.DelayedCall(1f, () =>
            {
                Time.timeScale = 0f;
            });
        }
        

        private void Start()
        {
            m_ContinueButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                Hide();
            });
            
        }
        
    }
}