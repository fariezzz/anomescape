using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Kunci cursor
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Biar gak muter 360 derajat

        // Putar kamera ke atas bawah
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Putar badan player ke kiri kanan
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            Debug.LogWarning("playerBody belum di-assign di Inspector!");
        }
    }
}
