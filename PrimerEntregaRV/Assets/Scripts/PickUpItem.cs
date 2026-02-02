using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isHeld = false;
    public bool isInspecting = false;

    [Header("Inspección")]
    public Vector3 inspectionOffset = new Vector3(0, 0, 0.4f);
    public float rotationSpeed = 600f;

    [Header("Sujetar en brazos")]
    public Vector3 holdOffset = new Vector3(0f, -0.35f, 0.6f);

    [Tooltip("Rotación inicial al inspeccionar (para que el frente del modelo mire a la cámara)")]
    public Vector3 inspectionRotationEuler = new Vector3(0f, 180f, 0f);

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isInspecting && Input.GetMouseButton(0)) // 0 = Click izquierdo
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }
    }

    // 🔎 MODO INSPECCIÓN
    public void OnInspect(Transform cameraTransform)
    {
        isInspecting = true;
        isHeld = false;

        if (rb) rb.isKinematic = true;

        transform.SetParent(cameraTransform);
        transform.localPosition = inspectionOffset;
        transform.localRotation = Quaternion.Euler(inspectionRotationEuler);
    }

    // ✋ MODO CARGANDO (EN BRAZOS)
    public void OnPickup(Transform holdPoint)
    {
        isInspecting = false;
        isHeld = true;

        if (rb) rb.isKinematic = true;

        transform.SetParent(holdPoint);

        // IMPORTANTE: posición EXACTA del HoldPoint
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDrop()
    {
        isHeld = false;
        isInspecting = false;

        transform.SetParent(null);
        if (rb) rb.isKinematic = false;
    }
}

