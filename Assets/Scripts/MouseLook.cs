using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] bool invert = false;
    [SerializeField] float sensitivity = 10.0f;


    [SerializeField] Camera playerCamera;
    [SerializeField] Transform playerBody;

    private float rotationX = 0;
    private float rotationY = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;


        rotationY += mouseX; // Rotate around the Y-axis
        if (invert)
        {
            rotationX -= -Input.GetAxis("Mouse Y") * sensitivity;
        }
        else
        {
            rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
        }
        // rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
    }
}