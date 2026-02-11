using UnityEngine;

public class DoorMessageTrigger : MonoBehaviour
{
    public GameObject messagePanel; 
    public DoorController door; 

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


