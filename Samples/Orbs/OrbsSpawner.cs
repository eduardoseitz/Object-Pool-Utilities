using UnityEngine;
using UnityEngine.InputSystem;
using DevPenguin.ObjectPoolUtilities;

public class OrbsSpawner : MonoBehaviour
{
    public InputActionReference clickInput;
    public GameObject redOrbPrefab;
    public Rigidbody2D greenOrbPrefab;
    
    private void Update()
    {
        if (clickInput.action.WasPressedThisFrame())
        {
            Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            // Old way to instantiate.
            //Instantiate(redOrbPrefab, _mousePosition, Quaternion.identity);
            //Rigidbody2D greenOrbPrefab = Instantiate(greenOrbPrefab, _mousePosition, Quaternion.identity).GetComponent<Rigidbody2D>();
            //greenOrbPrefab.gravityScale = 1;
            
            // Pooled way to instantiate.
            PoolManager.Instance.SpawnObject(redOrbPrefab, new Vector3(_mousePosition.x, _mousePosition.y, 0), Quaternion.identity, (int)ObjectPoolTypes.RedOrb);
            Rigidbody2D greenOrb = PoolManager.Instance.SpawnObject(greenOrbPrefab, new Vector3(_mousePosition.x, _mousePosition.y, 0), Quaternion.identity, (int)ObjectPoolTypes.GreenOrb);
            greenOrb.gravityScale = 1;
        }
    }
}

public enum ObjectPoolTypes{
    RedOrb,
    GreenOrb
}
