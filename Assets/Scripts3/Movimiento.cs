using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float gravity = -9.81f;

    [Header("Mouse")]
    public float mouseSensitivity = 0.1f;
    public Transform cameraPivot;

    [Header("Head Bob")]
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation = 0f;
    private float bobTimer = 0f;
    private Vector3 cameraInitialPos;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isRunning;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        cameraInitialPos = cameraPivot.localPosition;
    }

    void Update()
    {
        ReadInput();
        Look();
        Move();
        HeadBob();
    }

    void ReadInput()
    {
        moveInput = Keyboard.current != null 
            ? new Vector2(
                (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
                (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0)
              )
            : Vector2.zero;

        lookInput = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        isRunning = Keyboard.current.leftShiftKey.isPressed;
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void HeadBob()
    {
        if (controller.velocity.magnitude > 0.1f && controller.isGrounded)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;

            cameraPivot.localPosition = cameraInitialPos + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTimer = 0f;
            cameraPivot.localPosition = Vector3.Lerp(
                cameraPivot.localPosition,
                cameraInitialPos,
                Time.deltaTime * 5f
            );
        }
    }
}