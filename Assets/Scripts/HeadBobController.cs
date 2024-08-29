using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    public CharacterController controller;
    public Player player;

    public Vector3 restPosition;
    private new GameObject camera;

    [Header("Settings")]
    public float bobSpeed = 4.8f;
    public float bobAmount = 0.05f;

    private float timer = Mathf.PI / 2;

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }


    private void Update()
    {
        ChangeBobSpeedByMovements();
    }


    private void ChangeBobSpeedByMovements()
    {

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (player.IsSprinting())
            {
                bobSpeed = 7f;
                bobAmount = 0.10f;
            }
            
            else if (player.IsCrouching())
            {
                bobSpeed = 3f; 
            }
            else
            {
                bobSpeed = 4.8f; 
            }

            timer += bobSpeed * Time.deltaTime;

            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount,
            restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z);
            camera.transform.localPosition = newPosition;
        }
        else
        {
            timer = Mathf.PI / 2;
        }

        if (timer > Mathf.PI * 2)
        {
            timer = 0;
        }

    }
}
