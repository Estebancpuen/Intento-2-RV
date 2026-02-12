using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 20f;

    public Vector3 ShakeOffset { get; private set; }

    private bool shaking;

    public void StartShake()
    {
        shaking = true;
    }

    public void StopShake()
    {
        shaking = false;
        ShakeOffset = Vector3.zero;
    }

    void Update()
    {
        if (!shaking)
        {
            ShakeOffset = Vector3.zero;
            return;
        }

        float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) - 0.5f;
        float y = Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f;

        ShakeOffset = new Vector3(x, y, 0) * shakeIntensity;
    }
}


