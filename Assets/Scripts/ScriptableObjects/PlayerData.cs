using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private int maxStamina = 100;


    public float SprintSpeed => sprintSpeed;
    public float CrouchSpeed => crouchSpeed;
    public float WalkSpeed => walkSpeed;
    public float Gravity => gravity;
    public float JumpHeight => jumpHeight;
    public int MaxStamina => maxStamina;
}
