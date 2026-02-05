using UnityEngine;
using System.Collections;

public class FlickerLightOnWin : MonoBehaviour
{
    public Light targetLight;
    public float flickerDuration = 3f;
    public float minIntensity = 0.2f;
    public float maxIntensity = 2f;
    public float flickerSpeed = 0.05f;

    private bool isFlickering = false;

    public void StartFlicker()
    {
        if (!isFlickering)
            StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        isFlickering = true;
        float timer = 0f;

        while (timer < flickerDuration)
        {
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;
        }

        targetLight.intensity = maxIntensity;
        isFlickering = false;
    }
}
