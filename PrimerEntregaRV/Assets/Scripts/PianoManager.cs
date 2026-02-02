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
    [Header("Límite de intentos")]
    public int maxNotesBeforeEscape = 2;

    public void RegisterKeyPress(string noteName)
    {
        if (!dollIsSeated)
        {
            Debug.Log("La muñeca no está mirando, el puzzle no avanza.");
            return;
        }

        playerInput.Add(noteName);

        Debug.Log("Nota tocada: " + noteName);

        // 🎯 Si aún no ha llegado al límite, no evaluamos todavía
        if (playerInput.Count < maxNotesBeforeEscape)
        {
            return;
        }

        // 🔍 Ahora sí evaluamos cuando ya tocó las 2 notas
        bool sequenceCorrect = true;

        for (int i = 0; i < correctSequence.Length; i++)
        {
            if (i >= playerInput.Count || playerInput[i] != correctSequence[i])
            {
                sequenceCorrect = false;
                break;
            }
        }

        if (sequenceCorrect)
        {
            Debug.Log("¡Secuencia correcta, puerta abierta!");
        }
        else
        {
            Debug.Log("Secuencia incorrecta… la niña se fue.");
            HandleMistake();
        }
    }



    void HandleMistake()
    {
        Debug.Log("Secuencia incorrecta… la niña se asustó.");

        playerInput.Clear();
        dollIsSeated = false;

        TeleportDoll();
    }

    void TeleportDoll()
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[randomIndex];

        // Soltarla de cualquier estado (mano o asiento)
        PickUpItem itemScript = dollNPC.GetComponent<PickUpItem>();
        if (itemScript != null)
        {
            itemScript.OnDrop();
        }

        dollNPC.transform.SetParent(null);
        dollNPC.transform.position = spawn.position;
        dollNPC.transform.rotation = spawn.rotation;

        // Avisar al jugador que ya no la tiene
        PlayerInteraction player = Object.FindFirstObjectByType<PlayerInteraction>();
        if (player != null)
        {
            player.ResetHeldItem();
        }

        Debug.Log("La niña se ha escapado a otro lugar...");
    }



}
