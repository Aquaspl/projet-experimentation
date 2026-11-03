using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                         // The player to follow
    public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f); // Point to orbit around (roughly head height)

    [Header("Camera")]
    public float distance = 4f;                      // Set to 0 for first person
    public float sensitivity = 0.15f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    private float yaw;
    private float pitch;

    void Start()
    {
        // Start looking in the same direction as the camera currently faces
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        LockCursor(true);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Mouse mouse = Mouse.current;
        Keyboard kb = Keyboard.current;

        // Escape frees the cursor, clicking locks it again
        if (kb != null && kb.escapeKey.wasPressedThisFrame) LockCursor(false);
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) LockCursor(true);

        // Only rotate while the cursor is locked
        if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch -= delta.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Orbit around the target
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + targetOffset;
        transform.position = pivot - rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }

    void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}