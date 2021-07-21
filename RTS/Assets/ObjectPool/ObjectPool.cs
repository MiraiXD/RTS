using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KK.ObjectPool
{
    /// <summary>
    /// Pools prefab that contain a PoolableObject script
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        /// <summary>
        /// Prefab
        /// </summary>
        [SerializeField] private PoolableObject pooledPrefab;
        /// <summary>
        /// How many copies should be created?
        /// </summary>
        [SerializeField] private int amount = 100;
        private Queue<PoolableObject> pool;

        public void Init()
        {
            pool = new Queue<PoolableObject>();
            for (int i = 0; i < amount; i++)
            {
                pool.Enqueue(CreatePoolableObject());
            }
        }
        /// <summary>
        /// Returns the first available object from pool. If there's no objects left, will create a new one
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <returns></returns>
        public PoolableObject GetFromPool(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            PoolableObject poolableObject;
            if (pool.Count > 0)
            {
                poolableObject = pool.Dequeue();
            }
            else
            {
                poolableObject = CreatePoolableObject();
            }
            poolableObject.transform.SetParent(parent);
            poolableObject.transform.localPosition = localPosition;
            poolableObject.transform.localRotation = localRotation;
            poolableObject.gameObject.SetActive(true);
            return poolableObject;
        }
        /// <summary>
        ///  Returns a given PoolableObject to the pool. Don't use it, instead grab a reference to PoolableObject of your object and use its public method ReturnToPool
        /// </summary>
        /// <param name="poolableObject"></param>
        public void ReturnToPool(PoolableObject poolableObject)
        {
            poolableObject.transform.SetParent(transform);
            poolableObject.transform.localPosition = Vector3.zero;
            poolableObject.transform.localRotation = Quaternion.identity;
            poolableObject.gameObject.SetActive(false);
            pool.Enqueue(poolableObject);
        }
        private PoolableObject CreatePoolableObject()
        {
            PoolableObject poolableObject = Instantiate(pooledPrefab, transform);
            poolableObject.transform.localPosition = Vector3.zero;
            poolableObject.transform.localRotation = Quaternion.identity;
            poolableObject.gameObject.SetActive(false);
            poolableObject.SetOriginPool(this);
            poolableObject.gameObject.name = pooledPrefab.name + "(PoolableObject)";
            return poolableObject;

        }
    }
}
