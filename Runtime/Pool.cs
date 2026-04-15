using UnityEngine;

namespace DevPenguin.ObjectPoolUtilities
{
    [System.Serializable]
    public class Pool
    {
        #region Declarations
        
        [SerializeField] private string label;
        [SerializeField] private int minSize = 10;
        [SerializeField] private bool hasMaxSize = true;
        [SerializeField] private int maxSize = 100;
        
        #endregion

        #region Getters and Setters
        
        public string Label => label;
        
        public int MinSize => minSize;

        public bool HasMaxSize => hasMaxSize;

        public int MaxSize => maxSize;
        
        public GameObject Empty { get; set; }
        
        #endregion
    }
}