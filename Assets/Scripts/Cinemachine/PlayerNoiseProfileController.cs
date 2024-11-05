using UnityEngine;
using Cinemachine;
using System.Collections;

public class PlayerNoiseProfileController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerVirtualCamera;
    [SerializeField] private NoiseSettings walkNoiseProfile;
    [SerializeField] private NoiseSettings sprintNoiseProfile;
    [SerializeField] private NoiseSettings crouchNoiseProfile;
    

    private Player player;
    private CinemachineBasicMultiChannelPerlin noiseComponent;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        noiseComponent = playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        SetNoiseProfileBasedOnMovement();
    }

    private void SetNoiseProfileBasedOnMovement()
    {
        if (player.IsSprinting() && player.IsCharacterMoving())
        {
            noiseComponent.m_NoiseProfile = sprintNoiseProfile;
        }

        if (player.IsCharacterWalking())
        {
            noiseComponent.m_NoiseProfile = walkNoiseProfile;       
        }

        if (player.IsCrouching() && player.IsCharacterMoving())
        {
            noiseComponent.m_NoiseProfile = crouchNoiseProfile;
        }

        else if(!player.IsCharacterMoving())
        {
            noiseComponent.m_NoiseProfile = null;
        }    
    }

}



