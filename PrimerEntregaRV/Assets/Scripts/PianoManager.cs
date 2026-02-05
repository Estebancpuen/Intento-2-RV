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
    public bool puzzleSolved = false;

    public bool autoPlaying = true;
    public float autoPlayDelay = 0.5f; // tiempo entre notas

    public List<int> autoSequence = new List<int>();
    public int numberOfAutoNotes = 6;
    [Header("Piano Físico")]

   
    [Header("Teclas del Piano")]
    public List<PianoKey> pianoKeys = new List<PianoKey>();

    public DollStateManager dollState;

    [Header("Puerta de salida")]
    public DoorController doorController;




    void Start()
    {
        GenerateAutoSequence();
        StartCoroutine(AutoPlayRoutine());
        
    }



    void GenerateAutoSequence()
    {
        autoSequence.Clear();

        if (pianoKeys.Count == 0)
        {
            Debug.LogWarning("No hay teclas asignadas al PianoManager");
            return;
        }

        for (int i = 0; i < numberOfAutoNotes; i++)
        {
            autoSequence.Add(Random.Range(0, pianoKeys.Count));
        }
    }


    IEnumerator AutoPlayRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (autoPlaying)
        {
            if (pianoKeys.Count == 0) yield break;

            int randomIndex = Random.Range(0, pianoKeys.Count);

            // 👻 Presiona la tecla en modo automático
            pianoKeys[randomIndex].Press(true);

            yield return new WaitForSeconds(autoPlayDelay);
        }
    }


    public void OnPlayerSit()
    {
        StopAllCoroutines(); // corta lo que esté tocando
        StartCoroutine(FinishAutoPlay());
        autoPlaying = false;
    }

    IEnumerator FinishAutoPlay()
    {
        autoPlaying = true;

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, pianoKeys.Count);

            // 👻 Ahora la tecla se presiona sola correctamente
            pianoKeys[index].Press(true);

            yield return new WaitForSeconds(1.5f);
        }

        autoPlaying = false; // turno del jugador
    }


    public void RegisterKeyPress(string noteName)
    {
        if (!dollIsSeated || puzzleSolved)
            return;

        playerInput.Add(noteName);

        int currentIndex = playerInput.Count - 1;

        string pressed = noteName.Trim().ToLower();
        string expected = correctSequence[currentIndex].Trim().ToLower();

        Debug.Log("Nota presionada: " + pressed + " | Esperada: " + expected);

        // ❌ Si se equivoca en cualquier punto
        if (pressed != expected)
        {
            Debug.Log("Nota incorrecta… la niña se fue.");
            HandleMistake();
            return;
        }

        // ✅ Si completó TODA la secuencia correctamente
        if (playerInput.Count == correctSequence.Length)
        {
            puzzleSolved = true;
            doorController.DesbloquearPuerta();
            GameManager.instance.Victoria();
            // Reemplazar el uso obsoleto de FindObjectOfType con FindFirstObjectByType
            FindFirstObjectByType<FlickerLightOnWin>().StartFlicker();
            Debug.Log("¡Secuencia correcta, puerta abierta!");
            playerInput.Clear();

        }
    }




    void HandleMistake()
    {
        playerInput.Clear();
        dollIsSeated = false;

        if (dollState != null)
            dollState.RegisterMistake(); // 👈 AQUÍ pierde una parte

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
