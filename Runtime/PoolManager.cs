using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DevPenguin.ObjectPoolUtilities
{
    public class PoolManager : MonoBehaviour
    {
        #region Declarations

        private const string TAG = "PoolManager";
        public static PoolManager Instance;
        
        [Header("Debug Settings")]
        [SerializeField] private bool areLogsEnabled;
        [Space(2f)]
        
        [Header("Pool Settings")]
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private Pool[] pools;
        [Space(2f)]
        
        private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
        private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

        #endregion
        
        #region MonoBehaviour Methods
        /// <summary>
        /// Awake is called when the script instance is being loaded, before Start.
        /// </summary>
        private void Awake()
        {
            // If this is the only game manager on scene
            if (Instance == null)
            {
                // Store this object reference
                Instance = this;
            }
            else
            {
                // Destroy duplicate
                Destroy(gameObject);
            }
            
            // Make this scene persistant.
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            
            // Initialize dictionaries.
            _objectPools = new();
            _cloneToPrefabMap = new();
            
            // Make empty groups for each pool.
            SetupEmptyGroups();
        }
        
        #endregion

        #region Helper Methods

        internal T SpawnObject<T>(T typePrefab, Vector3 position, Quaternion rotation, int poolIndex) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, position, rotation, poolIndex);
        }
        
        internal GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, int poolIndex)
        { 
            return SpawnObject<GameObject>(prefab, position, rotation, poolIndex);
        }

        internal void ReturnObjectToPool(GameObject poolObject, int poolIndex)
        {
            if (_cloneToPrefabMap.TryGetValue(poolObject, out GameObject prefab))
            {
                if (poolIndex < pools.Length)
                {
                    poolObject.transform.SetParent(pools[poolIndex].Empty.transform);
                }
                else
                {
                    poolObject.transform.SetParent(transform);
                
                    if (areLogsEnabled)
                        Debug.LogWarning($"{TAG}: pool with index {poolIndex} does not exist!");
                }

                // If it finds out the object then release it.
                if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                {
                    pool.Release(poolObject);
                }
            }
            else
            {
                Debug.LogError($"{TAG}: Trying to release the object {poolObject.name} that isn't pooled from a pool!");
            }
        }
        
        /// <summary>
        /// Make empty groups for each pool.
        /// </summary>
        private void SetupEmptyGroups()
        {
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            for (int i = 0; i < pools.Length; i++)
            {
                pools[i].Empty = new GameObject("Pool " + pools[i].Label);
                pools[i].Empty.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                pools[i].Empty.transform.SetParent(transform);
                pools[i].Empty.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, int poolIndex)
        {
            // Set up new pool.
            ObjectPool<GameObject> newPool = new(
                createFunc:() => CreateObject(prefab, position, rotation, poolIndex),
                actionOnGet: GetObject,
                actionOnRelease: ReleaseObject,
                actionOnDestroy: DestroyObject);
            
            // Add new pool to the dictionary containing all pools.
            _objectPools.Add(prefab, newPool);
        }

        /// <summary>
        /// 
        /// </summary>
        private GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, int poolIndex)
        {
            // Make sure OnEnable and Awake doesn't get triggered by disabling the prefab before instantiate.
            prefab.SetActive(false);

            // Instantiate new object
            GameObject newObject = Instantiate(prefab, position, rotation);
            if (poolIndex < pools.Length)
            {
                newObject.transform.SetParent(pools[poolIndex].Empty.transform);
                newObject.name = $"{pools[poolIndex].Label}({poolIndex})";
            }
            else
            {
                newObject.transform.SetParent(transform);
                
                if (areLogsEnabled)
                    Debug.LogWarning($"{TAG}: pool with index {poolIndex} does not exist!");
            }
            
            // Reactivate prefab.
            prefab.SetActive(true);

            return newObject;
        }

        /// <summary>
        /// Retrieves an object from pool.
        /// </summary>
        private void GetObject(GameObject poolObject)
        {
            // TODO: Call an event when object is retrieved from pool.
        }

        /// <summary>
        /// Returns object to pool.
        /// </summary>
        private void ReleaseObject(GameObject poolObject)
        {
            // TODO: Call an event when object is returned to pool.
            poolObject.SetActive(false);
        }
        
        /// <summary>
        /// Destroys and removes an object from pool.
        /// </summary>
        private void DestroyObject(GameObject poolObject)
        {
            // TODO: Call an event when object is destroyed and removed from pool.
            if (_cloneToPrefabMap.ContainsKey(poolObject))
            {
                _cloneToPrefabMap.Remove(poolObject);
            }
        }

        /// <summary>
        /// Spawn a new generic object.
        /// </summary>
        private T SpawnObject<T>(GameObject prefab, Vector3 position, Quaternion rotation, int poolIndex) where T : Object
        {
            // If pool does not exist create one. 
            if (!_objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab, position, rotation, poolIndex);
            }
            // Else get one or instantiate a new one.
            GameObject newObject = _objectPools[prefab].Get();
            if (newObject)
            {
                if (!_cloneToPrefabMap.ContainsKey(newObject))
                {
                    _cloneToPrefabMap.Add(newObject, prefab);
                }
                
                newObject.transform.SetPositionAndRotation(position, rotation);
                newObject.SetActive(true);

                if (typeof(T) == typeof(GameObject))
                {
                    return newObject as T;
                }
                
                T component = newObject.GetComponent<T>();
                if (!component){
                    Debug.LogError($"{TAG}: {newObject.name} is a null component!");
                    return null;
                }
                return component;
            }
            return null;
        }
        
        #endregion
    }
}
