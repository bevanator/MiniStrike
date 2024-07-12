using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
namespace ProjectC.Core
{
    public class DamageIndicator : MonoBehaviour  
    {
        [SerializeField] private ParticleSystem m_ImpactParticle;
        [SerializeField] private List<Renderer> m_Renderers = new();
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private Color m_HighlightColor;
        [SerializeField] private Color m_DeathColor;
        [SerializeField] private Shootable m_Shootable;
        private Tweener _scaleTween;

        private void Start()
        {
            if (!m_Shootable) m_Shootable = GetComponent<Shootable>();
            m_Shootable.DamageReceived += ShowImpact;
        }

        private void OnDestroy()
        {
            m_Shootable.DamageReceived -= ShowImpact;
        }

        private void ShowImpact()
        {
            // m_ImpactParticle.Play();
            _scaleTween.Kill(); 
            transform.localScale = Vector3.one;
            _scaleTween = transform.DOPunchScale(0.05f * Vector3.one, 0.1f, 1);
            foreach (Renderer renderer in m_Renderers)   
            {
                foreach (Material material in renderer.materials)
                {
                    material.DOKill();
                    material.EnableKeyword("_EMISSION");
                    material.SetColor(EmissionColor, m_HighlightColor);
                    material.DOColor(Color.black, EmissionColor, 0.5f);
                }
            }
        }

        public void Death(Action Oncomplete = null)
        {
            foreach (Renderer renderer in m_Renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.DOKill();
                    material.EnableKeyword("_EMISSION");
                    material.DOColor(m_DeathColor, EmissionColor, 0.5f).OnComplete(delegate
                    {
                        Oncomplete?.Invoke();
                    });
                    
                }
            }
        }
    }
}