using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DevPenguin.ObjectPoolUtilities
{
    public class PoolManager : MonoBehaviour
    {
        #region Declarations

        private const string TAG = "PoolManager";
        // public static PoolManager Instance;
        //
        // [Header("Debug Settings")]
        // [SerializeField] private bool areLogsEnabled;
        // [Space(2f)]
        
        [Header("Pool Settings")]
        [SerializeField] private bool dontDestroyOnLoad = true;
        //[SerializeField] private PoolGroup[] poolGroups;
        [Space(2f)]
        
        private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
        private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

        //public PoolGroup[] PoolGroups => poolGroups;

        public enum PoolType
        {
            GameObjects,
            ParticleSystem,
            Sounds
        }

        public static PoolType poolType;
        
        #endregion
        
        #region MonoBehaviour Methods
        /// <summary>
        /// Awake is called when the script instance is being loaded, before Start.
        /// </summary>
        private void Awake()
        {
            // // If this is the only game manager on scene
            // if (Instance == null)
            // {
            //     // Store this object reference
            //     Instance = this;
            //     
            //     // Destroy duplicate
            //     Destroy(gameObject);
            // }
            //
            // // Make this scene persistant.
            // if (dontDestroyOnLoad)
            //     DontDestroyOnLoad(gameObject);
            
            // Initialize dictionaries.
            _objectPools = new();
            _cloneToPrefabMap = new();
            
            // Make empty groups for each pool.
            SetupEmptyGroups();
        }
        
        #endregion

        #region Helper Methods

        /// <summary>
        /// Make empty groups for each pool.
        /// </summary>
        private void SetupEmptyGroups()
        {
            // transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            // for (int group = 0; group < PoolGroups.Length; group++)
            // {
            //     PoolGroups[group].Empty = new GameObject($"{PoolGroups[group].Label} PoolGroup");
            //     PoolGroups[group].Empty.transform.SetParent(transform);
            //     PoolGroups[group].Empty.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            //     for (int pool = 0; pool < PoolGroups[group].Pools.Length; pool++)
            //     {
            //         PoolGroups[group].Pools[pool].Empty = new GameObject($"{PoolGroups[group].Pools[pool].Label} Pool");
            //         PoolGroups[group].Pools[pool].Empty.transform.SetParent(PoolGroups[group].Empty.transform);
            //         PoolGroups[group].Pools[pool].Empty.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            //     }
            // }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects)
        {
            // Set up new pool.
            ObjectPool<GameObject> newPool = new(
                createFunc:() => CreateObject(prefab, position, rotation, poolType),
                actionOnGet: GetObject,
                actionOnRelease: ReleaseObject,
                actionOnDestroy: DestroyObject);
            
            // Add new pool to the dictionary containing all pools.
            _objectPools.Add(prefab, newPool);
        }

        /// <summary>
        /// 
        /// </summary>
        private static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects)
        {
            // Make sure OnEnable and Awake doesn't get triggered by disabling the prefab before instantiate.
            prefab.SetActive(false);

            // Instantiate new object
            GameObject newObject = Instantiate(prefab, position, rotation);
            //newObject.transform.SetParent(PoolManager.Instance.transform);
            // TODO: Move to correct empty.
            
            // Reactivate prefab.
            prefab.SetActive(true);

            return newObject;
        }

        /// <summary>
        /// Retrieves an object from pool.
        /// </summary>
        private static void GetObject(GameObject poolObject)
        {
            // TODO: Call event when object is retrieved from pool.
        }

        /// <summary>
        /// Returns object to pool.
        /// </summary>
        private static void ReleaseObject(GameObject poolObject)
        {
            // TODO: Call event when object is returned to pool.
            poolObject.SetActive(false);
        }
        
        /// <summary>
        /// Destroys and removes an object from pool.
        /// </summary>
        private static void DestroyObject(GameObject poolObject)
        {
            // TODO: Call event when object is destroyed and removed from pool.
            if (_cloneToPrefabMap.ContainsKey(poolObject))
            {
                _cloneToPrefabMap.Remove(poolObject);
            }
        }

        /// <summary>
        /// Spawn a new generic object.
        /// </summary>
        private static T SpawnObject<T>(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType) where T : Object
        {
            // If pool does not exist create one. 
            if (!_objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab, position, rotation,  poolType);
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
        
        public static T SpawnObject<T>(T typePrefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, position, rotation, poolType);
        }
        
        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType)
        { 
            return SpawnObject<GameObject>(prefab, position, rotation, poolType);
        }

        internal static void ReturnObjectToPool(GameObject poolObject, PoolType poolType = PoolType.GameObjects)
        {
            if (_cloneToPrefabMap.TryGetValue(poolObject, out GameObject prefab))
            {
                // TODO: Set correct empty parent.
                //poolObject.transform.SetParent(PoolManager.Instance.transform);

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
        
        #endregion
    }
}
