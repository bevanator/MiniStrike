using DG.Tweening;
using EGF.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthUI : UIPanelBase
    {
        [SerializeField] private Health m_Health;
        [SerializeField] private Image m_HealthFillBar;
        [SerializeField] private Color m_HealthBarColor = Color.green;


        private void OnEnable()
        {
            m_Health.HealthModifiedEvent += OnHealthModifiedEvent;
        }


        private void OnDisable()
        {
            m_Health.HealthModifiedEvent -= OnHealthModifiedEvent;
        }

        private void OnHealthModifiedEvent(float value)
        {
            m_HealthFillBar.fillAmount = m_Health.CurrentHealth / m_Health.MaxHealth;
            m_HealthFillBar.DOKill();
            if (m_Health.CurrentHealth <= 0)
            {
                // gameObject.SetActive(false);
                return;
            }
            if(value>0) return;
            m_HealthFillBar.color = Color.white;
            m_HealthFillBar.DOColor(m_HealthBarColor, 0.3f);
        }
    }
}