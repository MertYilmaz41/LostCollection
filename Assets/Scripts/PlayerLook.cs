using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerLook : MonoBehaviour
{ 
    [SerializeField] private CinemachineVirtualCamera playerVirtualCamera;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private float xSensitivity = 15f;
    [SerializeField] private float ySensitivity = 15f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
     
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerVirtualCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);

    }
}
