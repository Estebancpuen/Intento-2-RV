using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float walkSpeed = 5f;
    public float gravity = -20f;
    public bool canMove = true; // NUEVA VARIABLE PARA CONTROLAR EL ESTADO

    [Header("Ajustes de Cámara")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- LA ROTACIÓN SIEMPRE FUNCIONA (para poder mirar el piano) ---
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // --- EL MOVIMIENTO SOLO FUNCIONA SI canMove ES TRUE ---
        if (!canMove) return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkSpeed * Input.GetAxis("Horizontal");

        Vector3 horizontalMove = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded)
        {
            moveDirection.y = -2f;
        }
        else
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        moveDirection.x = horizontalMove.x;
        moveDirection.z = horizontalMove.z;

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
