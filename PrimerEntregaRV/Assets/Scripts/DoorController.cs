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

        LockDoor(); 
    }

    void LockDoor()
    {
        rb.isKinematic = true;       
        hinge.useLimits = true;
    }

    public void DesbloquearPuerta()
    {
        if (unlocked) return;

        unlocked = true;

        rb.isKinematic = false;       
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse); 

        Debug.Log("Puerta desbloqueada");
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }
}


