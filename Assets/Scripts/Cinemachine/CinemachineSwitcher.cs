using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction action;
    [SerializeField] private CinemachineVirtualCamera playerWalkCam;
    [SerializeField] private CinemachineVirtualCamera playerSprintCam;
    [SerializeField] private NoiseSettings sprintNoiseProfile;


    private bool isPlayerWalkCamLive = true;

    void Start()
    {
        action.performed += ctx => SwitchCameraPriority();
    }

    private void LateUpdate()
    {
        playerSprintCam.transform.position = playerWalkCam.transform.position;
        playerSprintCam.transform.rotation = playerWalkCam.transform.rotation;
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void SwitchCameraPriority()
    {
        if (isPlayerWalkCamLive)
        {
            playerWalkCam.Priority = 0;
            playerSprintCam.Priority = 1;
        }
        else
        {
            playerWalkCam.Priority = 1;
            playerSprintCam.Priority = 0;
        }

        isPlayerWalkCamLive = !isPlayerWalkCamLive;
    }

}
