using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 20f;

    private Vector3 originalPos;
    private bool shaking;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void StartShake()
    {
        shaking = true;
    }

    public void StopShake()
    {
        shaking = false;
        transform.localPosition = originalPos;
    }

    void Update()
    {
        if (!shaking) return;

        float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) - 0.5f;
        float y = Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f;

        transform.localPosition = originalPos + new Vector3(x, y, 0) * shakeIntensity;
    }
}

