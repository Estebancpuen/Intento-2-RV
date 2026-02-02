using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float walkSpeed = 5f;
    public float gravity = -20f;
    public bool canMove = true;

    [Header("Ajustes de Cámara")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        if (characterController == null || !characterController.enabled) return;

        PickUpItem doll = Object.FindFirstObjectByType<PickUpItem>();
        if (doll != null && doll.isInspecting)
        {
            // Opcional: Liberar el cursor para que no se mueva la cámara pero sí la muñeca
            Cursor.lockState = CursorLockMode.Locked; // Mantener bloqueado para rotación
            Cursor.visible = false;
            return;
        }

        // Si no estamos inspeccionando, volvemos a bloquear el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 1. ROTACIÓN
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;


        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        // Rotación vertical de la cámara
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Rotación horizontal del cuerpo (CON R MAYÚSCULA)
        transform.Rotate(Vector3.up * mouseX);

        // 2. MOVIMIENTO
        HandleMovement();
    }

    void HandleMovement()
    {

        if (!characterController.enabled) return;

        // Aplicamos gravedad siempre para que el controller detecte el suelo correctamente
        if (characterController.isGrounded)
        {
            moveDirection.y = -2f;
        }
        else
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        if (!canMove)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkSpeed * Input.GetAxis("Horizontal");

        // Guardamos la Y de la gravedad antes de calcular la dirección horizontal
        float lastY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection.y = lastY;

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
