using UnityEngine;

public class DoorMessageTrigger : MonoBehaviour
{
    public GameObject messagePanel;
    public DoorController door;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip lockedDoorClip;

    private bool hasPlayedSound = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!door.IsUnlocked())
        {
            messagePanel.SetActive(true);

            if (!hasPlayedSound && audioSource != null && lockedDoorClip != null)
            {
                audioSource.PlayOneShot(lockedDoorClip);
                hasPlayedSound = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        messagePanel.SetActive(false);
        hasPlayedSound = false; // 🔥 se resetea al salir
    }
}




