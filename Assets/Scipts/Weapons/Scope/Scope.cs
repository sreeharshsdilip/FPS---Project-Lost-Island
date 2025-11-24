using UnityEngine;

public class Scope : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    
    [SerializeField] private float zoomedFOV = 30f;
    [SerializeField] private float normalFOV = 60f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleScope();
        }
    }

    private void ToggleScope()
    {
        if (playerCamera.fieldOfView == normalFOV)
        {
            playerCamera.fieldOfView = zoomedFOV;
        }
        else
        {
            playerCamera.fieldOfView = normalFOV;
        }
    }
}
