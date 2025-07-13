using UnityEngine;

public class ViewCameraController : MonoBehaviour
{
    public float sensitivity = 2f;
    private Vector2 rotation = Vector2.zero;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rotation.x += Input.GetAxis("Mouse X") * sensitivity;
            rotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        }
    }
}
