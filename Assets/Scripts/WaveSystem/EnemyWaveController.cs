using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enemy;
using Other;
using ProjectC.UI;
using UnityEngine;
using ZombieRoad.UI;
using Random = UnityEngine.Random;

namespace WaveSystem
{
    public class EnemyWaveController : MonoBehaviour
    {
        private int _enemiesKilled;
        private int _currentWaveNumber;
        private int _waveCount = 1;
        private WaveData _currentWave;
        [SerializeField] private WaveDataList m_WaveDataList;
        [SerializeField] private List<Transform> m_SpawnPoints;
        [SerializeField] private RectTransform m_MsgPos;
        private float _currentDamageMultiplier;
        private float _currentFireRateFactor;
        
        private void OnEnable()
        {
            EnemyBase.EnemyDeadEvent += OnEnemyDeadEvent;
        }

        private void OnDisable()
        {
            EnemyBase.EnemyDeadEvent -= OnEnemyDeadEvent;
        }

        private void OnEnemyDeadEvent()
        {
            ScoreManager.AddToKillScore();
            _enemiesKilled++;
            if (_enemiesKilled >= _currentWave.GetTotalNumberOfEnemies)
            {
                _enemiesKilled = 0;
                _currentWaveNumber++;
                if (_currentWaveNumber >= 4) _currentWaveNumber = Random.Range(1,4);
                GlobalSlideUpMessage.ShowMessage("Wave "+ (_waveCount-1) + " Complete", m_MsgPos.transform.position);
                DOVirtual.DelayedCall(2.5f, () => StartWaveByIndex(_currentWaveNumber));
            }
        }

        private void Start()
        {
            DOVirtual.DelayedCall(4f, () => StartWaveByIndex(_currentWaveNumber)).SetUpdate(false);
        }

        private void StartWaveByIndex(int index)
        {
            _currentWave = m_WaveDataList.GetWaveByIndex(index);
            GlobalSlideUpMessage.ShowMessage("WAVE " + _waveCount, m_MsgPos.transform.position);
            DOVirtual.DelayedCall(1f, () =>
            {
                GlobalSlideUpMessage.ShowMessage("<color=yellow><size=200>GO!", m_MsgPos.transform.position);
            });
            ScoreManager.AddToWaveScore();
            _waveCount++;
            StartCoroutine(nameof(StartWaveCoroutine));
        }

        private IEnumerator StartWaveCoroutine()
        {
            yield return new WaitForSeconds(_currentWave.WaveStartDelay);
            foreach (SpawnUnit unit in _currentWave.SpawnUnits)
            {
                for (int i = 0; i < unit.Count; i++)
                {
                    int randomSpawnIndex = unit.SpawnPointList[Random.Range(0, unit.SpawnPointList.Count)];
                    EnemyBase enemy = Instantiate(unit.EnemyPrefab, m_SpawnPoints[randomSpawnIndex].position, Quaternion.identity, transform);
                    if (enemy is EnemySoldier enemySoldier)
                    {
                        HandleDifficultyIncreaseWithWaveCount(unit);
                        enemySoldier.Init(unit.WeaponIndex, 3f, _currentDamageMultiplier, _currentFireRateFactor);
                    }
                    yield return new WaitForSeconds(unit.Interval);
                }
                float appropriateDelay = unit.StartAfterPreviousUnit ? (unit.Count * unit.Interval) : unit.InitialDelay;
                yield return new WaitForSeconds(appropriateDelay);
            }

            void HandleDifficultyIncreaseWithWaveCount(SpawnUnit unit)
            {
                _currentDamageMultiplier = unit.DamageMultiplier;
                _currentFireRateFactor = unit.FireRateFactor;
                _currentDamageMultiplier += _waveCount * 0.1f;
                _currentFireRateFactor -= _waveCount * 0.1f;
                _currentDamageMultiplier = Mathf.Clamp(_currentDamageMultiplier, unit.DamageMultiplier, 1f);
                _currentFireRateFactor = Mathf.Clamp(_currentFireRateFactor, 1f, 4f);
            }
        }
    }
}