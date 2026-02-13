using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float walkSpeed = 5f;
    public float gravity = -20f;
    public bool canMove = true;

    
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [SerializeField] private Animator handsAnimator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        
        PlayerInteraction interaction = GetComponent<PlayerInteraction>();
        if (interaction != null && interaction.IsInspecting())
        {
            return;
        }

        
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        
        if (characterController == null || !characterController.enabled) return;

        HandleMovement();
    }

    void HandleMovement()
    {

        if (!characterController.enabled) return;

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
            if (handsAnimator != null)
                handsAnimator.SetBool("isWalking", false);
            return;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkSpeed * Input.GetAxis("Horizontal");

        
        float lastY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection.y = lastY;

        bool isWalking = Mathf.Abs(curSpeedX) > 0.1f || Mathf.Abs(curSpeedY) > 0.1f;

        
        if (handsAnimator != null && !handsAnimator.GetBool("isSurprised"))
        {
            handsAnimator.SetBool("isWalking", isWalking);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null || rb.isKinematic) return;

        
        Vector3 pushDir = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

        if (pushDir.magnitude < 0.1f) return; 

        rb.AddForce(pushDir.normalized * 0.2f, ForceMode.Impulse);
    }

}
