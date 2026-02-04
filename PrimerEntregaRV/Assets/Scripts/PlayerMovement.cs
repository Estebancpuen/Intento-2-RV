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
        // 1. Si estamos inspeccionando, rotamos el objeto y salimos
        PlayerInteraction interaction = GetComponent<PlayerInteraction>();
        if (interaction != null && interaction.IsInspecting())
        {
            return;
        }

        // 2. ROTACIÓN DE CÁMARA (Debe ir ANTES del check del characterController)
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // 3. MOVIMIENTO (Solo si el controller está activo)
        if (characterController == null || !characterController.enabled) return;

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


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null || rb.isKinematic) return;

        // Dirección basada en el movimiento real del jugador
        Vector3 pushDir = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

        if (pushDir.magnitude < 0.1f) return; // evita empujones fantasmas

        rb.AddForce(pushDir.normalized * 0.2f, ForceMode.Impulse);
    }

}
