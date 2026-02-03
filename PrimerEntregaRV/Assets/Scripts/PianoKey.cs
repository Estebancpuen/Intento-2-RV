using UnityEngine;
using System.Collections;

public class PianoKey : MonoBehaviour
{
    public string noteName;
    public AudioClip noteSound; // Arrastra aquí el sonido de esta tecla
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private bool isPressing = false;
    public float pressDepth = 0.05f;

    private PianoManager pianoManager;

    void Start()
    {
        originalPosition = transform.localPosition;

        // Configuramos el AudioSource automáticamente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = noteSound;
        audioSource.playOnAwake = false;
        pianoManager = Object.FindFirstObjectByType<PianoManager>(); // Busca el manager en la escena
    }

    public void Press(bool fromAutoPlay = false)
    {
        if (!isPressing)
        {
            StartCoroutine(PressRoutine(fromAutoPlay));
        }
    }

    IEnumerator PressRoutine(bool fromAutoPlay)
    {
        isPressing = true;

        // 🔊 Sonido
        if (noteSound != null)
            audioSource.Play();

        // 🎯 SOLO avisamos al PianoManager si fue el jugador
        if (!fromAutoPlay)
            pianoManager.RegisterKeyPress(noteName);

        // Movimiento visual
        transform.localPosition = originalPosition + new Vector3(0, -pressDepth, 0);

        yield return new WaitForSeconds(0.15f);

        transform.localPosition = originalPosition;
        isPressing = false;
    }


    public void Press()
    {
        if (!isPressing)
        {
            StartCoroutine(PressRoutine());
        }
    }

    IEnumerator PressRoutine()
    {
        isPressing = true;

        // Reproducir sonido
        if (noteSound != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("¡Falta el archivo de sonido en la tecla: " + noteName);
        }
        pianoManager.RegisterKeyPress(noteName);
        // Movimiento visual
        transform.localPosition = originalPosition + new Vector3(0, -pressDepth, 0);
        Debug.Log("Tocaste: " + noteName);

        yield return new WaitForSeconds(0.15f);

        transform.localPosition = originalPosition;
        isPressing = false;
    }
}
