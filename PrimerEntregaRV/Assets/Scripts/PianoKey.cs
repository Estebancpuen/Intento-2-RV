using UnityEngine;
using System.Collections;

public class PianoKey : MonoBehaviour
{
    public string noteName;
    public AudioClip noteSound; 
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private bool isPressing = false;
    public float pressDepth = 0.05f;

    public MeshRenderer keyRenderer;
    public Material normalMaterial;
    public Material pressedMaterial;

    private PianoManager pianoManager;

    void Start()
    {
        originalPosition = transform.localPosition;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = noteSound;
        audioSource.playOnAwake = false;
        pianoManager = Object.FindFirstObjectByType<PianoManager>(); 
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

       
        if (keyRenderer != null && pressedMaterial != null)
            keyRenderer.material = pressedMaterial;

        
        if (noteSound != null)
            audioSource.Play();

        
        if (!fromAutoPlay)
            pianoManager.RegisterKeyPress(noteName);

        
        transform.localPosition = originalPosition + new Vector3(0, -pressDepth, 0);

        yield return new WaitForSeconds(0.15f);

        
        transform.localPosition = originalPosition;

        
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

        
        if (noteSound != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("¡Falta el archivo de sonido en la tecla: " + noteName);
        }
        pianoManager.RegisterKeyPress(noteName);
       
        transform.localPosition = originalPosition + new Vector3(0, -pressDepth, 0);
        Debug.Log("Tocaste: " + noteName);

        yield return new WaitForSeconds(0.15f);

        transform.localPosition = originalPosition;
        isPressing = false;
    }
}
