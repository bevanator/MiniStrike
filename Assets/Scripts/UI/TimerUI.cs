using DG.Tweening;
using EGF.UI;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Casinoventure.UI
{
    public class TimerUI : UIPanelBase
    {
        [SerializeField] private Image m_RingImage;
        private RectTransform _ringRt;
        
        private void Awake()
        {
            _ringRt = m_RingImage.GetComponent<RectTransform>();
            Hide();
        }
        [Button]
        public void StartRingTimer(float duration)
        {
            Show();
            m_RingImage.fillAmount = 1f;
            RectPopIn(_ringRt).OnComplete(() =>
            {
                m_RingImage.DOFillAmount(0f, duration).OnComplete(Hide);
            });
        }
        
    }
}