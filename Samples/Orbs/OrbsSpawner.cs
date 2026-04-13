using UnityEngine;
using UnityEngine.InputSystem;

public class OrbsSpawner : MonoBehaviour
{
    public InputActionReference clickInput;
    public GameObject orbPrefab;
    
    private void Update()
    {
        if (clickInput.action.WasPressedThisFrame())
        {
            Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Instantiate(orbPrefab, _mousePosition, Quaternion.identity);
        }
    }
}
