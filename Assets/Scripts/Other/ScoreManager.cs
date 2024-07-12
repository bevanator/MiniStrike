using UnityEngine;

namespace Other
{
    public class ScoreManager : MonoBehaviour
    {
        private int _totalEnemiesKilled;
        private int _totalWavesCleared;
        private static ScoreManager _instance;

        public static int TotalEnemiesKilled => _instance._totalEnemiesKilled;

        public static int TotalWavesCleared => _instance._totalWavesCleared;

        private void Awake()
        {
            _instance = this;
        }

        public static void AddToKillScore()
        {
            _instance._totalEnemiesKilled++;
        }
        public static void AddToWaveScore()
        {
            _instance._totalWavesCleared++;
        }
    }
}