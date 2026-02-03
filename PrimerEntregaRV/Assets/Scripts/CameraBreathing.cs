using UnityEngine;

public class CameraBreathing : MonoBehaviour
{
    public float breatheSpeed = 1.2f;
    public float breatheAmount = 0.015f;

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        timer += Time.deltaTime * breatheSpeed;
        float offset = Mathf.Sin(timer) * breatheAmount;

        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}

