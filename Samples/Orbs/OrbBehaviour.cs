using DevPenguin.ObjectPoolUtilities;
using UnityEngine;

public class OrbBehaviour : MonoBehaviour
{
    public float destroyAfterSeconds = 2.5f;
    
    private void Start()
    {
        Invoke(nameof(DestroyAfterSeconds), destroyAfterSeconds);
    }

    private void DestroyAfterSeconds()
    {
        // Old way to destroy.
        //Destroy(gameObject);
        
        // Pooled way to destroy.
        PoolManager.ReturnObjectToPool(gameObject);
    }
}
