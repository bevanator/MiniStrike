using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ProjectC.Pool
{
    public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected List<T> Pool;
        protected T Prefab;

        public void Initialize(T prefab, int initialSize)
        {
            Prefab = prefab;

            Pool = new List<T>();
            for (var i = 0; i < initialSize; i++)
            {
                var obj = Instantiate(prefab, transform, true);
                obj.gameObject.SetActive(false);
                Pool.Add(obj);
            }
        }

        public virtual T Get()
        {
            foreach (var obj in Pool)
            {
                if(obj.gameObject.activeInHierarchy) continue;
                obj.gameObject.SetActive(true);
                return obj;
            }

            var newObj = Object.Instantiate(Prefab);
            newObj.gameObject.SetActive(true);
            Pool.Add(newObj);
            return newObj;
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    }
}