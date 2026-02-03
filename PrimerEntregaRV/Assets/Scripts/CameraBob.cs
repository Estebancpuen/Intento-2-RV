using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public CharacterController controller;
    public float bobSpeed = 6f;
    public float bobAmount = 0.05f;
    public float idleAmount = 0.01f;

    private float defaultY;
    private float timer = 0;

    void Start()
    {
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        if (controller == null) return;

        float speed = controller.velocity.magnitude;

        if (controller.isGrounded && speed > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed;
            float bob = Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                defaultY + bob,
                transform.localPosition.z
            );
        }
        else
        {
            timer += Time.deltaTime * 1.5f;
            float idleBob = Mathf.Sin(timer) * idleAmount;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                defaultY + idleBob,
                transform.localPosition.z
            );
        }
    }
}

