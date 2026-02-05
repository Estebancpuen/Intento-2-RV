using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isHeld = false;
    public bool isInspecting = false;

    [Header("Inspección")]
    public Vector3 inspectionOffset = new Vector3(0, 0, 0.4f);
    public float rotationSpeed = 600f;

    [Tooltip("Rotación inicial al inspeccionar")]
    public Vector3 inspectStartRotationEuler = new Vector3(0f, 180f, 0f);

    [Header("Zoom al interactuar")]
    public float zoomAmount = 0.08f;
    public float zoomSpeed = 6f;

    [Header("Efecto poseída")]
    public float possessedShakeAmount = 0.00015f;
    public float possessedShakeSpeed = 12f;

    [Header("Regreso automático")]
    public float returnSpeed = 5f;

    private Quaternion initialInspectRotation;
    private Rigidbody rb;


    [Header("Solo objetos inspeccionables (ej: libro)")]
    public bool returnToOriginalPlace = false;
    public Transform originalPlacePoint;

    private bool returning = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

 

        // 🔁 MOVIMIENTO DE REGRESO (PRIMERO)
        if (returning && originalPlacePoint != null)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                originalPlacePoint.position,
                returnSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                originalPlacePoint.rotation,
                returnSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, originalPlacePoint.position) < 0.01f)
            {
                transform.position = originalPlacePoint.position;
                transform.rotation = originalPlacePoint.rotation;
                transform.SetParent(null);

                if (rb) rb.isKinematic = true;

                returning = false;
            }

            return; // ⛔ no ejecutar lógica de inspección mientras regresa
        }

        // 👁 SOLO si está inspeccionando
        if (!isInspecting) return;

        bool rotating = Input.GetMouseButton(0);

        // 🎯 ROTACIÓN
        if (rotating)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                initialInspectRotation,
                returnSpeed * Time.deltaTime
            );
        }

        // 🎥 POSICIÓN
        Vector3 targetPos = inspectionOffset;
        if (rotating)
            targetPos += new Vector3(0, 0, -zoomAmount);

        float shakeX = Mathf.Sin(Time.time * possessedShakeSpeed) * possessedShakeAmount;
        float shakeY = Mathf.Cos(Time.time * possessedShakeSpeed * 1.2f) * possessedShakeAmount;

        targetPos += new Vector3(shakeX, shakeY, 0);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            zoomSpeed * Time.deltaTime
        );
    }


    public void OnInspect(Transform cameraTransform)
    {
        isInspecting = true;
        isHeld = false;

        if (rb) rb.isKinematic = true;

        transform.SetParent(cameraTransform);
        transform.localPosition = inspectionOffset;

        initialInspectRotation = Quaternion.Euler(inspectStartRotationEuler);
        transform.localRotation = initialInspectRotation;
    }

    public void OnPickup(Transform holdPoint)
    {
        isInspecting = false;
        isHeld = true;

        if (rb) rb.isKinematic = true;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDrop()
    {
        isHeld = false;
        isInspecting = false;

        transform.SetParent(null);

        if (rb)
            rb.isKinematic = false;
    }


    public void FinishInspection()
    {
        isInspecting = false;
        isHeld = false;

        // 🔥 SOLTAR DEL PADRE ANTES DE REGRESAR
        transform.SetParent(null);

        if (!returnToOriginalPlace || originalPlacePoint == null)
        {
            OnDrop();
            return;
        }

        if (rb) rb.isKinematic = true; // que no interfiera la física
        returning = true;
    }

    public bool IsBusy()
    {
        return isInspecting || returning;
    }
}


