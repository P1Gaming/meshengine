using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float _speed = 20f;
    private float _mouseSensitivity = 10f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float flyInput = Input.GetKey(KeyCode.E) ? 1 : 0;
        flyInput += Input.GetKey(KeyCode.Q) ? -1 : 0;

        Vector3 movement = new Vector3(horizontalInput, flyInput, verticalInput);

        transform.Translate(movement * Time.deltaTime * _speed);

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector2 mouseInput = new Vector2(mouseX, mouseY);

            transform.Rotate(Vector3.up, mouseInput.x * _mouseSensitivity,Space.World);
            transform.Rotate(Vector3.right, -mouseInput.y * _mouseSensitivity,Space.Self);
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(WorldInfo.WorldDimensions/2,WorldInfo.WorldDimensions);
    }
}
