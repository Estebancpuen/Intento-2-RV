using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PianoManager : MonoBehaviour
{
    public string[] correctSequence;
    private List<string> playerInput = new List<string>();

    
    public bool dollIsSeated = false; 
    public Transform dollSeatPoint;   
    public GameObject dollNPC;
    public Transform[] spawnPoints;
    public bool puzzleSolved = false;

    public bool autoPlaying = true;
    public float autoPlayDelay = 0.5f; 

    public List<int> autoSequence = new List<int>();
    public int numberOfAutoNotes = 6;

    public List<PianoKey> pianoKeys = new List<PianoKey>();
    public DollStateManager dollState;
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

            pianoKeys[randomIndex].Press(true);

            yield return new WaitForSeconds(autoPlayDelay);
        }
    }


    public void OnPlayerSit()
    {
        StopAllCoroutines();
        StartCoroutine(FinishAutoPlay());
        autoPlaying = false;
    }

    IEnumerator FinishAutoPlay()
    {
        autoPlaying = true;

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, pianoKeys.Count);

            pianoKeys[index].Press(true);

            yield return new WaitForSeconds(1.5f);
        }

        autoPlaying = false; 
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

        if (pressed != expected)
        {
            Debug.Log("Nota incorrecta… la niña se fue.");
            HandleMistake();
            return;
        }

        if (playerInput.Count == correctSequence.Length)
        {
            puzzleSolved = true;
            doorController.DesbloquearPuerta();
            GameManager.instance.Victoria();
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
            dollState.RegisterMistake();

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

        PickUpItem itemScript = dollNPC.GetComponent<PickUpItem>();
        if (itemScript != null)
        {
            itemScript.OnDrop();
        }

        dollNPC.transform.SetParent(null);
        dollNPC.transform.position = spawn.position;
        dollNPC.transform.rotation = spawn.rotation;

        PlayerInteraction player = Object.FindFirstObjectByType<PlayerInteraction>();
        if (player != null)
        {
            player.ResetHeldItem();
        }

        Debug.Log("La niña se ha escapado a otro lugar...");
    }



}
