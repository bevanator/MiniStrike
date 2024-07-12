using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace WaveSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SpawnUnit", menuName = "Spawn Unit", order = 0)]
    public class SpawnUnit : ScriptableObject
    {
        public int Count;
        public float Interval;
        public float DamageMultiplier;
        public float FireRateFactor;
        public float InitialDelay;
        public bool StartAfterPreviousUnit;
        public EnemyBase EnemyPrefab;
        public int WeaponIndex;
        public List<int> SpawnPointList = new();
    }
}