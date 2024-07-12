using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WaveSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "WaveData", menuName = "Wave Data", order = 0)]
    public class WaveData : ScriptableObject
    {
        public float WaveStartDelay;
        [InlineEditor] public List<SpawnUnit> SpawnUnits = new();

        public int GetTotalNumberOfEnemies => SpawnUnits.Sum(unit => unit.Count);
    }
}