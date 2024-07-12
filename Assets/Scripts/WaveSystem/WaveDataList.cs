using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WaveSystem
{
    [CreateAssetMenu(fileName = "LevelWaveData", menuName = "Level Wave Data", order = 0)]
    public class WaveDataList : ScriptableObject
    {
        [SerializeField, InlineEditor] private List<WaveData> m_Waves = new();
        public WaveData GetWaveByIndex(int index) => m_Waves[index];
        
    }
}