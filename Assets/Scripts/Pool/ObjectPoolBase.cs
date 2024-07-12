// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using UnityEngine.Pool;
//
// namespace Pool
// {
//     public abstract class ObjectPoolBase<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
//     {
//         protected Dictionary<IPoolable, PoolObject<T>> _objectPoolDict;
//         
//         public void InitializePool(T poolablePrefab, int count = 0)
//         {
//             if (_objectPoolDict == null) _objectPoolDict = new Dictionary<IPoolable, PoolObject<T>>();
//             
//             if(_objectPoolDict.ContainsKey(poolablePrefab)) return;
//             
//             _objectPoolDict[poolablePrefab] = InitializePoolInternal(poolablePrefab, count);
//         }
//
//         private PoolObject<T> InitializePoolInternal(T poolPrefab, int defaultCount) 
//         {
//             PoolObject<T> poolObject = new PoolObject<T>
//             {
//                 Object = poolPrefab,
//                 DefaultCount = defaultCount
//             };
//
//             poolObject.Init(gameObject, poolPrefab.name);
//             return poolObject;
//         }
//         
//         
//         
//         public T Get(T poolablePrefab)
//         {
//             T poolable = _objectPoolDict[poolablePrefab].Get();
//             return poolable;
//         }
//
//         public void Release(T poolable)
//         {
//             _objectPoolDict[poolable.OriginalPoolPrefab].Release(poolable);
//         }
//         
//         
//         [Serializable]
//         public class PoolObject<T> where T : MonoBehaviour, IPoolable 
//         {
//             private GameObject parentHolder;
//             [SerializeField] private T m_Object;
//
//             private int _totalCreated;
//
//             public T Object
//             {
//                 get => m_Object;
//                 set => m_Object = value;
//             }
//
//             [SerializeField] private int m_DefaultCount;
//
//             public int DefaultCount
//             {
//                 get => m_DefaultCount;
//                 set => m_DefaultCount = value;
//             }
//
//             [SerializeField] private bool m_UseCustomParent;
//
//             [ShowIf("m_UseCustomParent")] [SerializeField]
//             private GameObject m_CustomParent;
//
//
//
//             private ObjectPool<T> _pool;
//
//             public void Init(GameObject holder, string name = "")
//             {
//                 if (m_UseCustomParent)
//                 {
//                     parentHolder = m_CustomParent;
//                 }
//                 else
//                 {
//                     parentHolder = new GameObject();
//                     if (name.Equals(""))
//                     {
//                         parentHolder.name = typeof(T).Name;
//                     }
//                     else
//                     {
//                         parentHolder.name = name;
//                     }
//
//                     parentHolder.transform.SetParent(holder.transform);
//                 }
//
//                 // Debug.Log(m_Object.name);
//
//                 _pool = new ObjectPool<T>(OnCreate, 
//                     o => OnGet(o), 
//                     o => OnRelease(o), 
//                     null, true, 5);
//
//                 List<T> list = new List<T>();
//                 for (int i = 0; i < m_DefaultCount; i++)
//                 {
//                     T t = _pool.Get();
//                     list.Add(t);
//                 }
//
//                 list.ForEach(x => _pool.Release(x));
//             }
//
//             private T OnCreate()
//             {
//                 T t = GameObject.Instantiate(m_Object, parentHolder.transform) as T;
//                 t.name = m_Object.name + " " + (++_totalCreated);
//                 t.OriginalPoolPrefab = Object;
//                 return t;
//             }
//
//             private T OnGet(T enemy)
//             {
//                 if (enemy == null) return null;
//                 enemy.gameObject.SetActive(true);
//                 return enemy;
//             }
//             
//             private T OnRelease(T enemy)
//             {
//                 if (enemy == null) return null;
//                 enemy.gameObject.SetActive(false);
//                 return enemy;
//             }
//
//             public T Get()
//             {
//                 return _pool.Get();
//             }
//
//             public void Release(T t)
//             {
//                 t.transform.SetParent(parentHolder.transform);
//                 _pool.Release(t);
//             }
//         }
//     }
// }