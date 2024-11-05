using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData originalPlayerData;
    private PlayerData playerDataInstance;

    [SerializeField] private LayerMask interactableLayerMask;

    private Vector3 playerVelocity;

    [SerializeField] private CinemachineVirtualCamera playerVirtualCamera; // camerayý cinemachine ile deðiþtir.

    private CharacterController characterController;
    public TMP_Text staminaText;

    [Header("Player Stats")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentStamina;

    [Header("Player Movement Status")]
    [SerializeField] private bool sprinting;   
    [SerializeField] private bool lerpCrouch = false;
    [SerializeField] private bool crouching;
    [SerializeField] private bool isGrounded;

    private float crouchTimer;
   
    private Coroutine regenCoroutine; 


    private Ray ray;
    private RaycastHit hit;
    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 1.8f;
    [SerializeField] private bool hitSomething;

    void Start()
    {
        playerDataInstance = Instantiate(originalPlayerData);
        currentSpeed = playerDataInstance.WalkSpeed;
        currentStamina = playerDataInstance.MaxStamina;
        sprinting = false;      
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        IsCharacterWalking();
        HandleCrouchLerp();
        DecreaseStamina();
        UpdateStaminaUI();               
        CastRay();
    }

    public void ProcessMovement(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        characterController.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity.y += playerDataInstance.Gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void JumpPerformed()
    {
        if (isGrounded && currentStamina >= 20)
        {
            playerVelocity.y = Mathf.Sqrt(playerDataInstance.JumpHeight * -2f * playerDataInstance.Gravity);
            currentStamina -= 20;
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            regenCoroutine = StartCoroutine(IncreaseStamina());
        }
    }


    public void SprintPerformed(InputAction.CallbackContext context)
    {
        
        sprinting = true;
        if(currentStamina >= 0 && !IsCrouching())
        {
            currentSpeed = playerDataInstance.SprintSpeed;
        }
        
    }

    public void SprintReleased(InputAction.CallbackContext context)
    {
        sprinting = false;
        if (!crouching)
        {
            currentSpeed = playerDataInstance.WalkSpeed;
            if (regenCoroutine == null)
            {
                regenCoroutine = StartCoroutine(IncreaseStamina());
            }
            
        }       
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0f;
        if (crouching)
        {
            sprinting = false; 
            currentSpeed = playerDataInstance.CrouchSpeed;
        }
        else
        {           
            currentSpeed = playerDataInstance.WalkSpeed; 
        }
        lerpCrouch = true;
    }

    private void DecreaseStamina()
    {
        if (currentSpeed == playerDataInstance.SprintSpeed && !IsCrouching() && IsCharacterMoving())
        {
            currentStamina -= 10 * Time.deltaTime * 2f;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                sprinting = false;
                currentSpeed = playerDataInstance.WalkSpeed;
                if (regenCoroutine == null)
                {
                    regenCoroutine = StartCoroutine(IncreaseStamina());
                }
            }
        }
    }

    private IEnumerator IncreaseStamina()
    {
        yield return new WaitForSeconds(2);
        while (currentStamina < playerDataInstance.MaxStamina &&  currentSpeed < playerDataInstance.SprintSpeed)
        {
            currentStamina += 10 * Time.deltaTime * 10f;
            yield return new WaitForSeconds(0.1f);
        }
        regenCoroutine = null;
    }

    private void HandleCrouchLerp()
    {
        isGrounded = characterController.isGrounded;
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;

            if (IsCrouching())
            {
                characterController.height = Mathf.Lerp(characterController.height, 1, p);
                currentSpeed = playerDataInstance.CrouchSpeed;
            }
            else
            {
                characterController.height = Mathf.Lerp(characterController.height, 2, p);
                currentSpeed = playerDataInstance.WalkSpeed;
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

    public bool IsCharacterWalking()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && !IsCrouching() && !IsSprinting())
        {
            currentSpeed = playerDataInstance.WalkSpeed;
            return true;
        }
        return false;
    }

    public bool IsCharacterMoving()
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
        ray = new Ray(playerVirtualCamera.transform.position, playerVirtualCamera.transform.forward);
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
