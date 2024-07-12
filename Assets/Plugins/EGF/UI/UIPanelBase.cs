using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EGF.UI
{
    public class UIPanelBase : MonoBehaviour
    {
        [SerializeField] protected Canvas m_Canvas;
        [SerializeField] protected CanvasGroup m_Panel;
        [SerializeField] private float m_FadeInDuration = 0.5f;
        [SerializeField] private float m_FadeOutDuration = 0.5f;
        
        protected virtual void Show()
        {
            m_Canvas.gameObject.SetActive(true);
            m_Panel.alpha = 0;
            m_Panel.DOFade(1f, m_FadeInDuration);
        }

        protected virtual void Hide()
        {
            m_Panel.DOFade(0f, m_FadeOutDuration).OnComplete(() => {
                m_Canvas.gameObject.SetActive(false);
                OnHideCompleted();
            });
        }
        
        protected Tweener RectPopIn(RectTransform rectTransform, float duration = 0.3f, float amount = 0.4f)
        {
            return rectTransform.DOPunchScale(amount * Vector3.one, duration, 1);
        }
        protected Tweener RectFadeOut(RectTransform rectTransform, float duration = 0.3f, float amount = 0.2f)
        {
            Vector3 defaultScale = Vector3.one;
            return rectTransform.DOScale(amount * Vector3.one, duration).OnComplete(() => transform.localScale = defaultScale).SetEase(Ease.OutBack, 3f);
        }

        protected virtual void OnHideCompleted()
        {
            
        }
    }
}