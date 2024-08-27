using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private LayerMask interactableLayerMask;

    private Vector3 playerVelocity;
    private new GameObject camera;
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
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        currentSpeed = playerData.WalkSpeed;
        currentStamina = playerData.MaxStamina;
        sprinting = false;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        IsCharacterWalking();
        HandleCrouchLerp();
        UpdateStaminaUI();
        CastRay();
        DecreaseStamina();
    }

    public void ProcessMovement(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        characterController.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity.y += playerData.Gravity * Time.deltaTime;
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
            playerVelocity.y = Mathf.Sqrt(playerData.JumpHeight * -2f * playerData.Gravity);
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
        //Stamina azalmasý için tuþa basým süresi þartý koy
        sprinting = true;
        if(currentStamina >= 0 && !IsCrouching())
        {
            currentSpeed = playerData.SprintSpeed;
        }
        Debug.Log("Sprint tuþuna basýldý");
    }

    public void SprintReleased(InputAction.CallbackContext context)
    {
        sprinting = false;
        currentSpeed = playerData.WalkSpeed;
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(IncreaseStamina());
        }
        Debug.Log("Sprint tuþu býrakýldý");
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

    private void DecreaseStamina()
    {
        if (currentSpeed == playerData.SprintSpeed && !IsCrouching() && IsCharacterMoving())
        {
            currentStamina -= 10 * Time.deltaTime * 2f;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                sprinting = false;
                currentSpeed = playerData.WalkSpeed;
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
        while (currentStamina < playerData.MaxStamina &&  currentSpeed < playerData.SprintSpeed)
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
            }
            else
            {
                characterController.height = Mathf.Lerp(characterController.height, 2, p);
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
