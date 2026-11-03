using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Rotation")]
    public float turnSpeed = 15f;
    [Tooltip("Optional. Leave empty to use the camera tagged MainCamera.")]
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        Transform cam = cameraTransform != null ? cameraTransform :
                        (Camera.main != null ? Camera.main.transform : null);
        if (cam == null) return;

        // Rotate the character to face the camera direction (yaw only)
        Quaternion targetRot = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // WASD / arrow keys
        float h = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
        float v = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
                - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);
        Vector3 input = new Vector3(h, 0f, v).normalized;

        Vector3 moveDir = transform.right * input.x + transform.forward * input.z;
        float speed = kb.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;

        // Gravity and jumping (isGrounded reflects the previous frame's Move call)
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (grounded && kb.spaceKey.wasPressedThisFrame)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        // One single Move call per frame so isGrounded stays reliable
        Vector3 motion = moveDir * speed;
        motion.y = verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }
}