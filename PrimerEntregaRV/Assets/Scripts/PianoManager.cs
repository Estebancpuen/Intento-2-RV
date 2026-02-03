using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
        if (!dollIsSeated)
        {
            Debug.Log("La muñeca no está mirando, el puzzle no avanza.");
            return;
        }

        playerInput.Add(noteName);
        Debug.Log("Input actual: " + string.Join(", ", playerInput));

        int currentIndex = playerInput.Count - 1;

        // ❌ Si esta nota no coincide con la secuencia correcta → error
        if (currentIndex >= correctSequence.Length ||
            !playerInput[currentIndex].Trim().ToLower()
            .Equals(correctSequence[currentIndex].Trim().ToLower()))
        {
            Debug.Log("Nota incorrecta… la niña se fue.");
            HandleMistake();
            return;
        }

        // ✅ Si completó toda la secuencia correctamente
        if (playerInput.Count == correctSequence.Length)
        {
            Debug.Log("¡Secuencia correcta, puerta abierta!");
            playerInput.Clear();
        }
    }





    void HandleMistake()
    {
        playerInput.Clear();
        dollIsSeated = false;

        StartCoroutine(MistakeSequence());
    }

    IEnumerator MistakeSequence()
    {
        DollGlitch glitch = dollNPC.GetComponent<DollGlitch>();
        if (glitch != null)
            yield return StartCoroutine(glitch.GlitchBeforeTeleport());

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
