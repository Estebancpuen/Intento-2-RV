using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isHeld = false;
    public bool isInspecting = false;

    [Header("Ajustes de Inspección")]
    public Vector3 inspectionOffset = new Vector3(0, 0, 0.6f);
    public float rotationSpeed = 450f; // Sube esto en el inspector también

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isInspecting)
        {
            // Usamos los ejes del mouse para rotar el objeto sobre sí mismo
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Rotación en ejes locales para que se sienta natural
            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }
    }

    public void OnInspect(Transform cameraTransform)
    {
        isInspecting = true;
        isHeld = false;
        if (rb) rb.isKinematic = true;

        transform.SetParent(cameraTransform);

        // ESTABILIDAD: Forzamos la posición local exacta frente a la cámara
        transform.localPosition = inspectionOffset;
        transform.localRotation = Quaternion.identity;
    }

    public void OnPickup(Transform holdPoint)
    {
        isInspecting = false;
        isHeld = true;

        transform.SetParent(holdPoint);
        // Forzamos posición y rotación cero respecto al nuevo padre (la mano)
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (rb) rb.isKinematic = true; // Asegurar que no se caiga por gravedad mientras se tiene
    }

    public void OnDrop()
    {
        isHeld = false;
        isInspecting = false;
        transform.SetParent(null);
        if (rb) rb.isKinematic = false;
    }
}
