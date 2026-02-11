using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RecordingCameraEffect : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Volume volume;

    [Header("Shake")]
    public float shakeIntensity = 0.015f;
    public float shakeSpeed = 1.3f;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float runFOV = 72f;
    public float fovSpeed = 6f;

    [Header("Glitch")]
    public float glitchChance = 0.005f;
    public float glitchIntensity = 0.08f;

    [Header("Night Vision")]
    public bool nightVision;

    Vector3 shakeOffset;

    FilmGrain filmGrain;
    ChromaticAberration chromatic;
    ColorAdjustments colorAdjust;
    Vignette vignette;

    void Start()
    {
        volume.profile.TryGet(out filmGrain);
        volume.profile.TryGet(out chromatic);
        volume.profile.TryGet(out colorAdjust);
        volume.profile.TryGet(out vignette);

        cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        HandleShake();
        HandleGlitch();
        HandleFOV();
        HandleNightVision();

        // 🔑 SOLO sumamos offset
        transform.localPosition = shakeOffset;
    }

    void HandleShake()
    {
        float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) - 0.5f;
        float y = Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f;

        shakeOffset = new Vector3(x, y, 0) * shakeIntensity;

        if (filmGrain != null)
            filmGrain.intensity.value = 0.35f;
    }

    void HandleGlitch()
    {
        if (Random.value < glitchChance)
        {
            shakeOffset += Random.insideUnitSphere * glitchIntensity;

            if (chromatic != null)
                chromatic.intensity.value = 0.45f;
        }
        else if (chromatic != null)
        {
            chromatic.intensity.value = Mathf.Lerp(
                chromatic.intensity.value, 0.1f, Time.deltaTime * 5f
            );
        }
    }

    void HandleFOV()
    {
        bool running = Input.GetKey(KeyCode.LeftShift);
        float target = running ? runFOV : normalFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView, target, Time.deltaTime * fovSpeed
        );
    }

    void HandleNightVision()
    {
        if (!nightVision)
        {
            if (colorAdjust != null)
                colorAdjust.colorFilter.value = Color.white;
            return;
        }

        if (colorAdjust != null)
        {
            colorAdjust.colorFilter.value = Color.green;
            colorAdjust.contrast.value = 25f;
            colorAdjust.postExposure.value = 1.2f;
        }

        if (vignette != null)
            vignette.intensity.value = 0.45f;
    }

    public void ToggleNightVision()
    {
        nightVision = !nightVision;
    }
}

