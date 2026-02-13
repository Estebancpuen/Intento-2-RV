using UnityEngine;
using System.Collections;

public class Particulas : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public ParticleSystem particles;
    public AudioSource audioSource;

    [Header("Timing")]
    public float duration = 5f;

    [Header("Random Events")]
    public float randomMinTime = 40f;
    public float randomMaxTime = 90f;

    [SerializeField] private Animator handsAnimator;

    bool isPlaying;
    Coroutine currentRoutine;

    void Start()
    {
        if (particles != null)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (audioSource != null)
            audioSource.Stop();

        StartCoroutine(RandomParticleRoutine());
    }

    void Update()
    {
        // 🎯 SIEMPRE seguir al jugador mientras está activo
        if (isPlaying && player != null)
        {
            transform.position = player.position;
        }
    }

    // 🔥 LLAMADO DESDE LAS LUCES
    public void PlayFromLights()
    {
        if (isPlaying) return;

        currentRoutine = StartCoroutine(PlayEvent());
    }

    IEnumerator PlayEvent()
    {
        isPlaying = true;

        // 🔑 Aparecer EXACTAMENTE en el jugador
        if (player != null)
            transform.position = player.position;

        // 🔥 ANIMACIÓN DE SORPRESA
        if (handsAnimator != null)
            handsAnimator.SetBool("isSurprised", true);

        // ▶ Partículas
        if (particles != null)
            particles.Play();

        // 🔊 Audio SOLO cuando las partículas están activas
        if (audioSource != null)
            audioSource.Play();

        yield return new WaitForSeconds(duration);

        StopParticles();
    }

    void StopParticles()
    {
        if (particles != null)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (handsAnimator != null)
            handsAnimator.SetBool("isSurprised", false);

        isPlaying = false;
    }

    // 👻 EVENTOS ALEATORIOS
    IEnumerator RandomParticleRoutine()
    {
        while (true)
        {
            float wait = Random.Range(randomMinTime, randomMaxTime);
            yield return new WaitForSeconds(wait);

            if (!isPlaying)
                currentRoutine = StartCoroutine(PlayEvent());
        }
    }


}

