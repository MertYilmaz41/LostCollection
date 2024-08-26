using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
   
    private PlayerControls playerControl;
    private PlayerControls.OnGroundActions onGround;
    private Player player;
    private PlayerLook look;


    private void Update()
    {
        player.ProcessMovement(onGround.Walk.ReadValue<Vector2>());

    }

    void Awake()
    {
        player = GetComponent<Player>();
        look = GetComponent<PlayerLook>();
        playerControl = new PlayerControls();
        onGround = playerControl.OnGround;
        onGround.Jump.performed += context => player.Jump();
        onGround.Run.performed += context => player.SprintPerformed(context);
        onGround.RunFinish.performed += context => player.SprintReleased(context);
        onGround.Crouch.performed += context => player.Crouch();
        onGround.Interact.performed += context => player.Interact(context);
    }

    void LateUpdate()
    {       
        look.ProcessLook(onGround.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {    
        onGround.Enable();      
    }

    private void OnDisable()
    {     
        onGround.Disable();      
    }
}
