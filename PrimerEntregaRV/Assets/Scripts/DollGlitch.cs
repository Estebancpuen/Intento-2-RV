using UnityEngine;
using System.Collections;

public class DollGlitch : MonoBehaviour
{
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [Header("Intensidad del movimiento raro")]
    public float tiltAngle = 10f;          
    public float sideShift = 0.02f;        
    public float freezeTime = 0.4f;        
    public float tiltTime = 0.3f;
    public float shiftTime = 0.2f;

    void Start()
    {
        originalRotation = transform.rotation;
        originalPosition = transform.position;
    }

    public IEnumerator GlitchBeforeTeleport()
    {
        
        yield return new WaitForSeconds(freezeTime);

        
        transform.Rotate(Vector3.right, tiltAngle);
        yield return new WaitForSeconds(tiltTime);

        
        transform.position += transform.right * sideShift;
        yield return new WaitForSeconds(shiftTime);

        
        transform.rotation = originalRotation;
        transform.position = originalPosition;
    }
}


