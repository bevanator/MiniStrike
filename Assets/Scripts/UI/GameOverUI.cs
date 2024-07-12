using System;
using DG.Tweening;
using EGF.UI;
using EGF.Utilities;
using Other;
using ProjectC;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : UIPanelBase
    {
        [SerializeField] private Button m_RestartButton;
        [SerializeField] private Button m_ExitButton;
        [SerializeField] private TextMeshProUGUI m_ScoreText;
        

        private void Awake()
        {
            Hide();
        }

        private void OnEnable()
        {
            Player.OnPlayerDeath += OnPlayerDeath;
        } 
        
        private void OnDisable()
        {
            Player.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            Show();
            DOVirtual.DelayedCall(1f, () =>
            {
                m_ScoreText.text = "Total Enemies Killed:  <color=green>" 
                                   + ScoreManager.TotalEnemiesKilled 
                                   + "</color=green>"
                                   + "\nTotal  Waves Cleared: <color=green>"
                                   + ScoreManager.TotalWavesCleared;
                Time.timeScale = 0f;
            });
        }

        private void Start()
        {
            m_RestartButton.onClick.AddListener(() => { LoadMap(); });
            
            m_ExitButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                DOTween.KillAll();
                SceneManager.LoadScene(0);
            });
        }

        public static void LoadMap()
        {
            Time.timeScale = 1f;
            DOTween.KillAll();
            SceneManager.LoadScene(1);
        }
    }
}