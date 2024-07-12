using System;
using ProjectC.UI;
using UnityEngine;

namespace Pool
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;

        [SerializeField] private PopNumbersUI m_PopNumberUIPrefab;

        // [SerializeField] private EnemyPool m_EnemyPool;
        [SerializeField] private BulletPool m_BulletPool;
        [SerializeField] private PopNumberPool m_PopNumberPool;


        // public EnemyPool EnemyPool => m_EnemyPool;
        public BulletPool BulletPool => m_BulletPool;

        public PopNumberPool PopNumberPool => m_PopNumberPool;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            PopNumberPool.Initialize(m_PopNumberUIPrefab, 30);
        }
    }
}