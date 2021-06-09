using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JammoController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    Vector2 currentMovmentInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    float rotationFactorPerFrame=15;
    float runMultiplier = 3;

    int isWalkingHash;
    int isRunningHash;

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");


        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRunInput;
        playerInput.CharacterControls.Run.canceled += onRunInput;

    }

    void onRunInput(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    void onMovementInput(InputAction.CallbackContext ctx)
    {

        currentMovmentInput = ctx.ReadValue<Vector2>();
        currentMovement.x = currentMovmentInput.x;
        currentMovement.z = currentMovmentInput.y;
        currentRunMovement.x = currentMovmentInput.x * runMultiplier;
        currentRunMovement.z = currentMovmentInput.y * runMultiplier;

        isMovementPressed = currentMovmentInput.x != 0 || currentMovmentInput.y != 0;
    }
    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPressed||!isRunPressed)&&isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }
    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation=transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
           transform.rotation= Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame*Time.deltaTime);
        }

    }

    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -0.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity*Time.deltaTime;
            currentRunMovement.y += gravity*Time.deltaTime;
        }
    }

    void Update()
    {
        handleRotation();
        handleAnimation();
        handleGravity();
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else { 
        characterController.Move(currentMovement * Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
