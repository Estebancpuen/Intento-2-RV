using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float walkSpeed = 5f;
    public float gravity = -20f; // Aumentada para una sensación más sólida

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

        // Bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- LÓGICA DE ROTACIÓN (MIRAR) ---
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // --- LÓGICA DE MOVIMIENTO (CAMINAR) ---
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkSpeed * Input.GetAxis("Horizontal");

        // Calculamos el movimiento horizontal
        Vector3 horizontalMove = (forward * curSpeedX) + (right * curSpeedY);

        // --- LÓGICA DE GRAVEDAD Y SUELO ---
        if (characterController.isGrounded)
        {
            // Fuerza constante hacia abajo para evitar "trepar"
            moveDirection.y = -2f;
        }
        else
        {
            // Aplicar gravedad si está en el aire
            moveDirection.y += gravity * Time.deltaTime;
        }

        // Combinamos: El movimiento horizontal es nuevo cada frame, 
        // pero el vertical (y) se mantiene/acumula para la gravedad.
        moveDirection.x = horizontalMove.x;
        moveDirection.z = horizontalMove.z;

        // Mover el controlador
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
