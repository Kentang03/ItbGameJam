using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform root;
    public Transform target;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        root.position = target.position;
    }

}
