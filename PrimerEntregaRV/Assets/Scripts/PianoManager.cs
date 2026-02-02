using UnityEngine;
using System.Collections.Generic;

public class PianoManager : MonoBehaviour
{
    public string[] correctSequence;
    private List<string> playerInput = new List<string>();

    [Header("Asientos y Estados")]
    public bool dollIsSeated = false; // El piano consultará esto
    public Transform dollSeatPoint;   // Arrastra el punto del cilindro pequeño
    public GameObject dollNPC;
    public Transform[] spawnPoints;

    public void RegisterKeyPress(string noteName)
    {
        // BLOQUEO: Si la niña no está sentada, no procesa la nota
        if (!dollIsSeated)
        {
            Debug.Log("La muñeca no está mirando, el puzzle no avanza.");
            return;
        }

        playerInput.Add(noteName);

        for (int i = 0; i < playerInput.Count; i++)
        {
            if (playerInput[i] != correctSequence[i])
            {
                HandleMistake();
                return;
            }
        }

        if (playerInput.Count == correctSequence.Length)
        {
            Debug.Log("¡Puerta abierta!");
        }
    }

    void HandleMistake()
    {
        playerInput.Clear();
        dollIsSeated = false; // La niña ya no está sentada porque va a teletransportarse
        TeleportDoll();
    }

    void TeleportDoll()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        dollNPC.transform.position = spawnPoints[randomIndex].position;
        dollNPC.transform.rotation = spawnPoints[randomIndex].rotation;

        PickUpItem itemScript = dollNPC.GetComponent<PickUpItem>();
        if (itemScript != null)
        {
            itemScript.OnDrop();
        }

        // NUEVO: Avisar al PlayerInteraction que ya no tiene a la muñeca
        PlayerInteraction player = Object.FindFirstObjectByType<PlayerInteraction>();
        if (player != null)
        {
            // Necesitarás hacer 'heldItem' público en PlayerInteraction o crear un método ResetHeldItem()
            player.ResetHeldItem();
        }

        Debug.Log("La muñeca se ha marchado...");
    }


}
