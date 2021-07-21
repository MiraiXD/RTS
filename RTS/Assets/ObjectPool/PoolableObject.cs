using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace KK.ObjectPool
{
    /// <summary>
    /// Should reside on every object that is supposed to be a prefab for pooling
    /// </summary>
    public class PoolableObject : MonoBehaviour
    {
        private ObjectPool originPool;
        public event Action<PoolableObject> onReturnToPool;
        public void SetOriginPool(ObjectPool originPool)
        {
            this.originPool = originPool;
        }
        /// <summary>
        /// Use it to easily return the object when not needed anymore, instead of Destroying it
        /// </summary>
        public void ReturnToPool()
        {
            originPool.ReturnToPool(this);
            onReturnToPool?.Invoke(this);
        }
    }
}
