using UnityEngine;
using System.Collections;

public class DollGlitch : MonoBehaviour
{
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [Header("Intensidad del movimiento raro")]
    public float tiltAngle = 10f;          // Cuánto se inclina
    public float sideShift = 0.02f;        // Cuánto se mueve lateralmente
    public float freezeTime = 0.4f;        // Tiempo quieta antes de moverse
    public float tiltTime = 0.3f;
    public float shiftTime = 0.2f;

    void Start()
    {
        originalRotation = transform.rotation;
        originalPosition = transform.position;
    }

    public IEnumerator GlitchBeforeTeleport()
    {
        // Se queda rígida
        yield return new WaitForSeconds(freezeTime);

        // Inclinación perturbadora (más pequeña ahora)
        transform.Rotate(Vector3.right, tiltAngle);
        yield return new WaitForSeconds(tiltTime);

        // Pequeño desplazamiento lateral
        transform.position += transform.right * sideShift;
        yield return new WaitForSeconds(shiftTime);

        // Volver a la pose normal antes de desaparecer
        transform.rotation = originalRotation;
        transform.position = originalPosition;
    }
}


