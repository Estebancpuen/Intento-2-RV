using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


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

    [Header("Distorsión de cámara")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float fishEyeIntensity = -0.4f;
    [SerializeField] private float distortionInTime = 0.5f;
    [SerializeField] private float distortionOutTime = 1f;

    private LensDistortion lensDistortion;



    [SerializeField] private Animator handsAnimator;

    bool isPlaying;
    Coroutine currentRoutine;

    void Start()
    {
        if (particles != null)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (audioSource != null)
            audioSource.Stop();

        if (globalVolume.profile.TryGet(out lensDistortion))
        {
            lensDistortion.intensity.value = 0f;
        }

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

        if (lensDistortion != null)
            StartCoroutine(FishEyeEffect(fishEyeIntensity));




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

        if (lensDistortion != null)
            StartCoroutine(ResetFishEye());



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

    IEnumerator FishEyeEffect(float target)
    {
        float startValue = lensDistortion.intensity.value;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / distortionInTime;
            lensDistortion.intensity.value = Mathf.Lerp(startValue, target, t);
            yield return null;
        }
    }

    IEnumerator ResetFishEye()
    {
        float startValue = lensDistortion.intensity.value;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / distortionOutTime;
            lensDistortion.intensity.value = Mathf.Lerp(startValue, 0f, t);
            yield return null;
        }
    }


}

