using DG.Tweening;
using Pool;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ProjectC.UI
{
    public class PopNumbersUI : MonoBehaviour, IPoolable
    {
        [SerializeField] private RectTransform m_AmountPanel;
        [SerializeField] private TextMeshProUGUI m_Amount;
        private CanvasGroup _canvasGroup;
        private float _initialPosY;
        private Vector3 _initialPosition;
        [SerializeField] private float m_Height = 100f;
        [SerializeField] private float m_Duration;
        

        private void Awake()
        {
            _canvasGroup = m_AmountPanel.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }
        private void FadeOut()
        {
            _canvasGroup.DOFade(1, m_Duration * 0.6f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                _canvasGroup.DOFade(0, m_Duration * 0.3f).SetEase(Ease.InSine).SetDelay(m_Duration * 0.2f);
            });
            m_AmountPanel.DOAnchorPosY(_initialPosition.y + m_Height, m_Duration * 8f).SetEase(Ease.OutSine)
                .OnStart(() =>
                {
                    RectPopIn(m_Amount.rectTransform, 0.2f, 0.5f);
                })
                .OnComplete(() =>
                {
                    m_AmountPanel.DOAnchorPosY(_initialPosition.y + m_Height * 2f, m_Duration).SetEase(Ease.InSine)
                        .OnComplete(ReleaseBackToPool);
                });
        }
        
        private void Init(string text)
        {
            m_AmountPanel.localPosition = transform.localPosition = Vector3.zero;
            m_Amount.text = text;
            _initialPosition = m_AmountPanel.localPosition = 2f * Vector3.up;
        }

        [Button]
        public void Pop(string text)
        {
            Init(text);
            FadeOut();
        }
        
        private Tweener RectPopIn(RectTransform rectTransform, float duration = 0.3f, float amount = 0.4f)
        {
            return rectTransform.DOPunchScale(amount * Vector3.one, duration, 1);
        }

        public void ReleaseBackToPool()
        {
            PoolManager.Instance.PopNumberPool.Release(this);
        }
        
    }
}