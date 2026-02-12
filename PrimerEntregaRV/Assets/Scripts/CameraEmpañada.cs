using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;

public class CameraEmpañada : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Volume volume;
    public RawImage fogImage;

    [Header("Movement")]
    public float timeToFog = 6f;

    [Header("Fog")]
    public float maxFogAlpha = 0.75f;
    public float fogFadeInSpeed = 0.8f;
    public float fogFadeOutSpeed = 0.3f;
    public float recoveryDelay = 3f; // ⬅ tiempo quieto antes de desvanecer

    [Header("Depth Of Field - Bokeh")]
    public float baseFocalLength = 24f;
    public float maxFocalLength = 80f;
    public float focalIncreaseSpeed = 8f;
    public float focalDecreaseSpeed = 6f;

    [Header("Fatigue")]
    public float fatigueDelay = 15f;     // tiempo sin efecto
    public float buildupTime = 8f;       // cuánto tarda en llegar al máximo
    public float recoverySpeed = 0.4f;

    float fatigue;      // 0 → 1
    float stopTimer;
    float holdTimer;



    [Header("Blur")]
    public float maxBlurRadius = 1.2f;

    float moveTimer;
    float currentFog;
   

    DepthOfField dof;

    void Start()
    {
        volume.profile.TryGet(out dof);

        if (dof != null)
        {
            dof.active = true;
            dof.focalLength.value = baseFocalLength;
        }

        fogImage.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        bool pressingW = Input.GetKey(KeyCode.W);

        if (pressingW)
        {
            holdTimer += Time.deltaTime;
            stopTimer = 0f;

            // Solo empieza después del delay
            if (holdTimer > fatigueDelay)
            {
                float t = (holdTimer - fatigueDelay) / buildupTime;
                fatigue = Mathf.Clamp01(t);
            }
        }
        else
        {
            holdTimer = Mathf.Max(holdTimer - Time.deltaTime * 2f, 0f);
            stopTimer += Time.deltaTime;

            if (stopTimer >= recoveryDelay)
            {
                fatigue = Mathf.MoveTowards(
                    fatigue,
                    0f,
                    Time.deltaTime * recoverySpeed
                );
            }
        }

        UpdateFogAndBlur(fatigue);
    }



    void UpdateFogAndBlur(float t)
    {
        fogImage.color = new Color(
            1, 1, 1,
            Mathf.Lerp(0f, maxFogAlpha, t)
        );

        if (dof != null)
        {
            dof.active = t > 0.01f;
            dof.focalLength.value = Mathf.Lerp(
                baseFocalLength,
                maxFocalLength,
                t
            );
        }
    }

}
