using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EGF.UI
{
    public class ButtonClickPop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform m_RootRect;
        [SerializeField] private Button m_Button;
        [SerializeField] private bool m_DisableWhenNotInteractable;

        [SerializeField] private float m_ClickScale = 0.9f;


        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_DisableWhenNotInteractable && !m_Button.interactable) return;
            m_RootRect.DOScale(m_ClickScale, 0.1f).SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_DisableWhenNotInteractable && !m_Button.interactable) return;
            m_RootRect.DOScale(1, 0.1f).SetUpdate(true);
        }
    }
}