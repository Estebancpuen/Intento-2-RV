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

    [Header("Materiales")]
    public MeshRenderer keyRenderer;
    public Material normalMaterial;
    public Material pressedMaterial;

    private PianoManager pianoManager;

    void Start()
    {
        originalPosition = transform.localPosition;

        // Configuramos el AudioSource automáticamente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = noteSound;
        audioSource.playOnAwake = false;

        // --- CONFIGURACIÓN PARA SONIDO ESPACIAL ---
        audioSource.spatialBlend = 1.0f; // 1.0 es 3D total, 0 es 2D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // El volumen cae con la distancia
        audioSource.minDistance = 0.01f;  // Se escucha al máximo a 1 metro
        audioSource.maxDistance = 0.1f; // A los 10 metros ya no se escucha nada
                                       // ------------------------------------------


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

        // 🎨 Cambiar a material presionado
        if (keyRenderer != null && pressedMaterial != null)
            keyRenderer.material = pressedMaterial;

        // 🔊 Sonido
        if (noteSound != null)
            audioSource.Play();

        // 🎯 Solo contar si es el jugador
        if (!fromAutoPlay)
            pianoManager.RegisterKeyPress(noteName);

        // ⬇ Movimiento hacia abajo
        transform.localPosition = originalPosition + new Vector3(0, -pressDepth, 0);

        yield return new WaitForSeconds(0.15f);

        // ⬆ Regresa a posición
        transform.localPosition = originalPosition;

        // 🎨 Volver al material normal
        if (keyRenderer != null && normalMaterial != null)
            keyRenderer.material = normalMaterial;

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
