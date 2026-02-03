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

    [Header("Zoom al interactuar")]
    public float zoomAmount = 0.08f;
    public float zoomSpeed = 6f;

    [Header("Efecto poseída")]
    public float possessedShakeAmount = 0.00015f;
    public float possessedShakeSpeed = 12f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isInspecting) return;

        bool rotating = Input.GetMouseButton(0);

        // ROTACIÓN solo con click
        if (rotating)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }

        // ZOOM SUAVE
        Vector3 targetPos = inspectionOffset;

        if (rotating)
            targetPos += new Vector3(0, 0, -zoomAmount); // se acerca a cámara

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            zoomSpeed * Time.deltaTime
        );

        if (isInspecting)
        {
            // Temblor poseído
            float shakeX = Mathf.Sin(Time.time * possessedShakeSpeed) * possessedShakeAmount;
            float shakeY = Mathf.Cos(Time.time * possessedShakeSpeed * 1.2f) * possessedShakeAmount;

            transform.localPosition = inspectionOffset + new Vector3(shakeX, shakeY, 0);

            // Rotación SOLO si el jugador mantiene click
            if (Input.GetMouseButton(0))
            {
                float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

                transform.Rotate(Vector3.up, -rotX, Space.World);
                transform.Rotate(Vector3.right, rotY, Space.World);
            }
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

