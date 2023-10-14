using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float _speed = 5f;
    private float _mouseSensitivity = 100f;

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

            transform.Rotate(Vector3.up, mouseInput.x * Time.deltaTime * _mouseSensitivity);
            transform.Rotate(Vector3.right, -mouseInput.y * Time.deltaTime * _mouseSensitivity);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(WorldInfo.WorldDimensions/2,WorldInfo.WorldDimensions);
    }
}