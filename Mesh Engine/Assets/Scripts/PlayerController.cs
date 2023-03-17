using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamTranform;
    [SerializeField] CharacterController playerController;

    [SerializeField] float rotationSensistivity = 100f;

    float xRotation;

    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float airSpeed;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float gravity;
    [SerializeField] float noClipSpeed;
    [SerializeField] Transform groundCheck;

    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;
    bool grounded;
    bool jump;
    bool sprint;
    bool noClip;
    Vector2 movementDirection;
    float defaultStepOffset;
    Vector2 mouse;

    private void OnEnable()
    {
        InputManager.moveAction += OnMove;
        InputManager.jumpAction += OnJump;
        InputManager.sprintAction += OnSprint;
        InputManager.noClipAction += OnNoClip;
        InputManager.lookAction += OnMouseLook;
        InputManager.scrollInventoryAction += OnScrollInventory;

        GameEvents.UpdateAction += OnUpdate;
    }

    private void OnDisable()
    {
        InputManager.moveAction -= OnMove;
        InputManager.jumpAction -= OnJump;
        InputManager.sprintAction -= OnSprint;
        InputManager.noClipAction -= OnNoClip;
        InputManager.lookAction -= OnMouseLook;
        InputManager.scrollInventoryAction -= OnScrollInventory;

        GameEvents.UpdateAction -= OnUpdate;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultStepOffset = playerController.stepOffset;
    }

    private void OnUpdate(float dT, bool gamePaused)
    {
        if (gamePaused)
            return;

        Rotate();

        if (noClip)
        {
            Vector3 lateralMovement = playerCamTranform.right * movementDirection.x;
            Vector3 forwardMovement = playerCamTranform.forward * movementDirection.y;

            transform.position += (forwardMovement + lateralMovement) * noClipSpeed * Time.deltaTime;
        }
        else
        {
            grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (!grounded)
                jump = false;

            if (grounded && velocity.y < 0)
            {
                velocity.y = -2f;
                playerController.stepOffset = defaultStepOffset;
            }

            float speed = 0;
            if (grounded)
                speed = sprint ? sprintSpeed : walkSpeed;
            else
                speed = airSpeed;

            Vector3 move = transform.right * movementDirection.x + transform.forward * movementDirection.y;
            playerController.Move(move * Time.deltaTime * speed);

            if (jump && grounded)
            {
                playerController.stepOffset = 0f;
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                jump = false;
            }
            velocity.y += gravity * Time.deltaTime;

            playerController.Move(velocity * Time.deltaTime);
        }
        GameEvents.UpdatePlayerPositionEvent?.Invoke(transform.position);
    }

    void Move()
    {

    }

    void Rotate()
    {
        xRotation -= mouse.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamTranform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouse.x);
    }

    public void OnMove(Vector2 input)
    {
        movementDirection = input;
    }

    private void OnMouseLook(Vector2 mouseInput)
    {
        mouse = mouseInput * Time.deltaTime * rotationSensistivity;

    }

    public void OnJump(bool input)
    {
        jump = true;
    }

    public void OnSprint(bool input)
    {
        sprint = input;
    }

    public void OnMenu(bool value)
    {

    }

    public void OnNoClip()
    {
        noClip = !noClip;
    }

    public void OnScrollInventory(float input)
    {
        print(input);
    }
}
