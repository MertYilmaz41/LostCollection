using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    private PlayerControls playerControl;
    private PlayerControls.OnGroundActions onGround;
    private Player player;
    private PlayerLook look;


    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Update()
    {
        player.ProcessMovement(onGround.Walk.ReadValue<Vector2>());

    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            playerControl = new PlayerControls();
        }
        player = GetComponent<Player>();
        look = GetComponent<PlayerLook>();    
        onGround = playerControl.OnGround;
        onGround.JumpStart.performed += context => player.JumpPerformed();     
        onGround.SprintStart.performed += context => player.SprintPerformed(context);
        onGround.SprintFinish.performed += context => player.SprintReleased(context);
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

    public Vector2 GetMouseDelta()
    {
        return onGround.Look.ReadValue<Vector2>();
    }
}
