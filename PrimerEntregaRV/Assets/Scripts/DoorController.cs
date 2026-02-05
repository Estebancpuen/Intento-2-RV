using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Rigidbody rb;
    private HingeJoint hinge;

    private bool unlocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();

        LockDoor(); // 👈 Empieza bloqueada
    }

    void LockDoor()
    {
        rb.isKinematic = true;        // No se mueve
        hinge.useLimits = true;
    }

    public void DesbloquearPuerta()
    {
        if (unlocked) return;

        unlocked = true;

        rb.isKinematic = false;       // Ahora sí tiene físicas
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse); // pequeño empujón

        Debug.Log("Puerta desbloqueada");
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }
}


