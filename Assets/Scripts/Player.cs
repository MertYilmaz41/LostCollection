using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private LayerMask interactableLayerMask;

    private new GameObject camera;
    public CharacterController controller;
    public TMP_Text staminaText;

    [SerializeField] private float currentSpeed;
    private int currentStamina;
    private Vector3 playerVelocity;

    private bool isGrounded;
    private bool sprinting = false;   
    private bool lerpCrouch = false;

    private bool crouching;
    private float crouchTimer;

    
    private Coroutine regenCoroutine;


    private Ray ray;
    private RaycastHit hit;
    private float rayDistance = 1.8f;
    private bool hitSomething;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        currentSpeed = playerData.WalkSpeed;
        currentStamina = playerData.MaxStamina;
    }

    void Update()
    {
        IsCharacterWalking();
        HandleCrouchLerp();
        UpdateStaminaUI();
        CastRay();
    }

    public void ProcessMovement(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;

        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity.y += playerData.Gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded && currentStamina >= 20)
        {
            playerVelocity.y = Mathf.Sqrt(playerData.JumpHeight * -2f * playerData.Gravity);
            currentStamina -= 20;
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            //regenCoroutine = StartCoroutine(RegenStamina());
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        bool isShiftPressed = context.performed;
        bool isShiftReleased = context.canceled;

        if (isShiftPressed && !crouching && currentStamina > 0)
        {
            if (!sprinting)
            {
                sprinting = true;
                currentSpeed = playerData.SprintSpeed;

                // Eðer stamina yenileme süreci varsa, durdur.
                if (regenCoroutine != null)
                {
                    StopCoroutine(regenCoroutine);
                    regenCoroutine = null;
                }

                // Sadece sprinting coroutinesi çalýþmýyorsa baþlat.
                if (!IsInvoking("SprintStaminaDrain"))
                {
                    StartCoroutine(SprintStaminaDrain());
                }
            }
        }
        else if (isShiftReleased)
        {
            sprinting = false;
            currentSpeed = playerData.WalkSpeed;

            // Eðer stamina yenilemesi baþlamamýþsa baþlat.
            if (regenCoroutine == null)
            {
                regenCoroutine = StartCoroutine(RegenStamina());
            }
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0f;
        if (crouching)
        {
            sprinting = false; // Disable sprinting when crouching
            currentSpeed = playerData.CrouchSpeed;
        }
        else
        {
            currentSpeed = playerData.WalkSpeed; // Reset speed to normal when standing up
        }
        lerpCrouch = true;
    }

    private IEnumerator SprintStaminaDrain()
    {
        while (IsSprinting() && currentStamina > 0)
        {
            currentStamina -= 5;
            yield return new WaitForSeconds(1f);

            if (currentStamina <= 0)
            {
                sprinting = false;
                currentSpeed = playerData.WalkSpeed;
                if (regenCoroutine != null)
                {
                    StopCoroutine(regenCoroutine);
                }
                regenCoroutine = StartCoroutine(RegenStamina());
            }
        }
    }

    private IEnumerator RegenStamina()  
    {      
        if (currentSpeed < playerData.SprintSpeed)
        {
            yield return new WaitForSeconds(2);
            while (currentStamina < playerData.MaxStamina)
            {
                currentStamina += 1000 / 100;
                yield return new WaitForSeconds(1f);
            }
        }
        
    }

    
    private void HandleCrouchLerp()
    {
        isGrounded = controller.isGrounded;
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;

            if (IsCrouching())
            {
                controller.height = Mathf.Lerp(controller.height, 1, p);
            }
            else
            {
                controller.height = Mathf.Lerp(controller.height, 2, p);
                currentSpeed = playerData.WalkSpeed;
            }

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

   

    private void UpdateStaminaUI()
    {
        staminaText.SetText("Stamina: " + currentStamina);
    }

    private bool IsCharacterWalking()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && !IsCrouching() && !IsSprinting())
        {
            currentSpeed = playerData.WalkSpeed;
            return true;
        }
        return false;
    }

    private bool IsCharacterMoving()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            return true;
        }
        return false;
    }

    public bool IsSprinting()
    {
        return sprinting;
    }

    public bool IsCrouching()
    {
        return crouching;
    }

    private void CastRay()
    {
        ray = new Ray(camera.transform.position, camera.transform.forward);
        hitSomething = Physics.Raycast(ray, out hit, rayDistance, interactableLayerMask, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, hitSomething ? Color.green : Color.red, 0f);
    }


    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && hitSomething)
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        
        }
        else if (context.performed && !hitSomething)
        {
            Debug.Log("F tuþuna basýldý ama obje interactable deðil");
        }
    }
}
