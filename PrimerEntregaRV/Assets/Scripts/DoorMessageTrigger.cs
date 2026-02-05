using UnityEngine;

public class DoorMessageTrigger : MonoBehaviour
{
    public GameObject messagePanel; // Arrastras aquí el panel del Canvas
    public DoorController door; // 👈 Arrastra aquí la puerta

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !door.IsUnlocked())
        {
            messagePanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            messagePanel.SetActive(false);
        }
    }
}


