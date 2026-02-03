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
    public float autoPlayDelay = 1.2f; // tiempo entre notas

    public List<int> autoSequence = new List<int>();
    public int numberOfAutoNotes = 6;
    [Header("Piano Físico")]

    
    public float keyPressDepth = 0.01f;
    public float keyReturnSpeed = 6f;
    [Header("Teclas del Piano")]
    public List<PianoKey> pianoKeys = new List<PianoKey>();




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

    void PlayKeySound(int index)
    {
        if (index < 0 || index >= keySounds.Count) return;

        if (keySounds[index] != null)
            keySounds[index].Play();
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

    void AnimateKeyPress(int index)
    {
        if (index < 0 || index >= pianoKeys.Count) return;

        Transform key = pianoKeys[index];
        StartCoroutine(KeyPressRoutine(key));
    }

    IEnumerator KeyPressRoutine(Transform key)
    {
        Vector3 startPos = key.localPosition;
        Vector3 pressedPos = startPos + Vector3.down * keyPressDepth;

        float t = 0;

        // Bajar
        while (t < 1)
        {
            t += Time.deltaTime * keyReturnSpeed;
            key.localPosition = Vector3.Lerp(startPos, pressedPos, t);
            yield return null;
        }

        t = 0;

        // Subir
        while (t < 1)
        {
            t += Time.deltaTime * keyReturnSpeed;
            key.localPosition = Vector3.Lerp(pressedPos, startPos, t);
            yield return null;
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

        // últimas notas lentas y tensas
        for (int i = 0; i < 3; i++)
        {
            int key = Random.Range(0, pianoKeys.Count);
            PlayKeySound(key);
            AnimateKeyPress(key);
            yield return new WaitForSeconds(1.5f);
        }

        autoPlaying = false; // 🎹 ahora sí, turno del jugador
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
