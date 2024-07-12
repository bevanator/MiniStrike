using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ZombieRoad.UI
{
    public class SlideUpMessage : MonoBehaviour
    {
        [SerializeField] private Canvas m_Canvas;
        
        [SerializeField] private TextMeshProUGUI m_TextToShow;
        [SerializeField] private float m_FadeInTime = 0.5f;
        [SerializeField] private float m_ElevationTime = 1f;
        [SerializeField] private float m_ElevationHeight = 200f;
        [SerializeField] private float m_FadeOutTime = 0.5f;
        [ReadOnly]
        [ShowInInspector] private float _initialPosY;
        
        private Sequence _textSequence;


        private void Start()
        {
            _initialPosY = m_TextToShow.rectTransform.anchoredPosition.y;
        }


        public void ShowMessage(string msg)
        {
            m_TextToShow.text = msg;
            ShowMessage();
        }

        [Button]
        public void ShowMessage()
        {
            m_Canvas.gameObject.SetActive(true);
            if(_textSequence.IsActive()) _textSequence.Kill();
            
            m_TextToShow.rectTransform.DOAnchorPosY(_initialPosY, 0f).SetUpdate(true);
            _textSequence = DOTween.Sequence();
            _textSequence.Append(m_TextToShow.DOColor(Color.white, m_FadeInTime));
            _textSequence.Join(m_TextToShow.rectTransform.DOAnchorPosY(_initialPosY + m_ElevationHeight, m_ElevationTime));
            _textSequence.AppendInterval(0.5f);

            _textSequence.Append(m_TextToShow.DOColor(Color.clear, m_FadeOutTime));
            _textSequence.OnComplete(() =>
            {
                m_TextToShow.rectTransform.DOAnchorPosY(_initialPosY, 0f);
                m_Canvas.gameObject.SetActive(false);
            });
            _textSequence.SetUpdate(true);
        }
    }
}