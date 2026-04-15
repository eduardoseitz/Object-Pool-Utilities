using DevPenguin.ObjectPoolUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbsSpawner : MonoBehaviour
{
    public InputActionReference clickInput;
    public Rigidbody2D orbPrefab;
    
    private void Update()
    {
        if (clickInput.action.WasPressedThisFrame())
        {
            Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            // Old way to instantiate.
            //Instantiate(orbPrefab, _mousePosition, Quaternion.identity);
            
            // Pooled way to instantiate.
            Rigidbody2D rigidbody2D = PoolManager.SpawnObject(orbPrefab, new Vector3(_mousePosition.x, _mousePosition.y, 0), Quaternion.identity, PoolManager.PoolType.GameObjects);
            rigidbody2D.gravityScale = 1;
        }
    }
}
