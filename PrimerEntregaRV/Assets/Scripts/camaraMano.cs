using UnityEngine;

public class camaraMano : MonoBehaviour
{
    [Header("Inertia")]
    public float positionStiffness = 120f;
    public float positionDamping = 18f;

    [Header("Hand Noise")]
    public float noiseAmplitude = 0.025f;
    public float noiseSpeed = 1.2f;

    [Header("Human Error")]
    public float driftStrength = 0.015f;

    Vector3 velocity;
    Vector3 noiseOffset;
    Vector3 driftOffset;

    void Update()
    {
        // 🎥 Ruido humano (Perlin, no seno)
        noiseOffset.x = (Mathf.PerlinNoise(Time.time * noiseSpeed, 0) - 0.5f) * noiseAmplitude;
        noiseOffset.y = (Mathf.PerlinNoise(0, Time.time * noiseSpeed) - 0.5f) * noiseAmplitude;

        // 😵 Micro errores humanos
        driftOffset = Vector3.Lerp(
            driftOffset,
            Random.insideUnitSphere * driftStrength,
            Time.deltaTime * 0.3f
        );

        Vector3 targetOffset = noiseOffset + driftOffset;

        // ⚖️ Spring (peso real)
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition,
            targetOffset,
            ref velocity,
            1f / positionStiffness,
            Mathf.Infinity,
            Time.deltaTime
        );
    }
}
