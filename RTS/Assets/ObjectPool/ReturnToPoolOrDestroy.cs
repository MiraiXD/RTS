using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.ObjectPool;
namespace ZombieApocalypseTest
{
    /// <summary>
    /// Composition based component for returning an object or destroying. Use it for projectiles and spells
    /// </summary>
    public class ReturnToPoolOrDestroy : MonoBehaviour
    {
        [SerializeField] private PoolableObject poolableObject;
        public void ReturnOrDestroy(GameObject gameObject)
        {
            if (poolableObject != null) poolableObject.ReturnToPool();
            else Destroy(gameObject);
        }
    }
}