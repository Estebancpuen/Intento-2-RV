using UnityEngine;
using System.Collections;

public class LightFlickerManager : MonoBehaviour
{
    [Header("Luces")]
    public Light[] spotLights;

    [Header("Flicker")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.05f;
    public float flickerDuration = 3f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    [Header("Audio (ya configurado en el Empty)")]
    public AudioSource flickerAudio;

    [Header("Timing")]
    public float minWaitTime = 10f;
    public float maxWaitTime = 25f;

    private float[] originalIntensities;
    private bool isRunning;

    void Start()
    {
        originalIntensities = new float[spotLights.Length];

        for (int i = 0; i < spotLights.Length; i++)
            originalIntensities[i] = spotLights[i].intensity;

        StartCoroutine(RandomFlickerLoop());
    }

    IEnumerator RandomFlickerLoop()
    {
        while (true)
        {
            float wait = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(wait);

            if (!isRunning)
                StartCoroutine(FlickerRoutine());
        }
    }

    IEnumerator FlickerRoutine()
    {
        isRunning = true;

        if (flickerAudio && !flickerAudio.isPlaying)
            flickerAudio.Play();

        cameraShake.StartShake();

        float timer = 0f;

        while (timer < flickerDuration)
        {
            foreach (Light l in spotLights)
                l.intensity = Random.Range(minIntensity, maxIntensity);

            timer += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        for (int i = 0; i < spotLights.Length; i++)
            spotLights[i].intensity = originalIntensities[i];

        cameraShake.StopShake();

        if (flickerAudio && flickerAudio.isPlaying)
            flickerAudio.Stop();

        isRunning = false;
    }
}


