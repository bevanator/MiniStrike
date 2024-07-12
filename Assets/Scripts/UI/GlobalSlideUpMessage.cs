using System;
using UnityEngine;
using ZombieRoad.UI;

namespace ProjectC.UI
{
    public class GlobalSlideUpMessage : SlideUpMessage
    {
        [SerializeField] private RectTransform m_Holder;
        
        private static GlobalSlideUpMessage Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public static void ShowMessage(string msg, Vector3 pos)
        {
            Instance.ShowMessage(msg);
            Instance.m_Holder.transform.position = pos;
        }
    }
}