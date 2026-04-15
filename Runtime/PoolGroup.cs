using UnityEngine;

namespace DevPenguin.ObjectPoolUtilities
{
    [System.Serializable]
    public class PoolGroup
    {
        #region Declarations

        [SerializeField] private string label;
        [SerializeField] private Pool[] pools;

        #endregion

        #region Getters and Setters
        
        public string Label => label;
        
        public Pool[] Pools => pools;
        
        public GameObject Empty { get; set; }

        #endregion
    }
}
