using System.Collections;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isHeld = false;
    public bool isInspecting = false;

    public Vector3 inspectionOffset = new Vector3(0, 0, 0.4f);
    public float rotationSpeed = 600f;

    
    public Vector3 inspectStartRotationEuler = new Vector3(0f, 180f, 0f);

    
    public float zoomAmount = 0.08f;
    public float zoomSpeed = 6f;

    
    public float possessedShakeAmount = 0.00015f;
    public float possessedShakeSpeed = 12f;

    
    public float returnSpeed = 5f;

    private Quaternion initialInspectRotation;
    private Rigidbody rb;


    
    public bool returnToOriginalPlace = false;
    public Transform originalPlacePoint;

    private bool returning = false;

    [Header("Inspection Audio")]
    public AudioSource inspectAudio;
    public AudioClip inspectClip;

    [Header("Idle Doll Sounds")]
    public AudioSource idleAudio;
    public AudioClip[] idleClips;
    public float minIdleSoundTime = 10f;
    public float maxIdleSoundTime = 30f;

    [Header("Inspection Audio Timing")]
    public float inspectMinDelay = 4f;
    public float inspectMaxDelay = 9f;

    private Coroutine inspectAudioRoutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(IdleSoundRoutine());
    }

    void Update()
    {

 
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

            return; 
        }

        
        if (!isInspecting) return;

        bool rotating = Input.GetMouseButton(0);

        
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

        if (inspectAudio && inspectClip)
            inspectAudioRoutine = StartCoroutine(InspectAudioRoutine());
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

        if (inspectAudioRoutine != null)
        {
            StopCoroutine(inspectAudioRoutine);
            inspectAudioRoutine = null;
        }

        if (inspectAudio && inspectAudio.isPlaying)
            inspectAudio.Stop();
    }


    public void FinishInspection()
    {
        isInspecting = false;
        isHeld = false;

        
        transform.SetParent(null);

        if (!returnToOriginalPlace || originalPlacePoint == null)
        {
            OnDrop();
            return;
        }

        if (rb) rb.isKinematic = true;
        returning = true;

        if (inspectAudioRoutine != null)
        {
            StopCoroutine(inspectAudioRoutine);
            inspectAudioRoutine = null;
        }

        if (inspectAudio && inspectAudio.isPlaying)
            inspectAudio.Stop();
    }

    public bool IsBusy()
    {
        return isInspecting || returning;
    }

    IEnumerator IdleSoundRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minIdleSoundTime, maxIdleSoundTime);
            yield return new WaitForSeconds(wait);

            if (!isInspecting && !isHeld && idleClips.Length > 0)
            {
                AudioClip clip = idleClips[Random.Range(0, idleClips.Length)];
                idleAudio.clip = clip;
                idleAudio.Play();
            }
        }
    }

    IEnumerator InspectAudioRoutine()
    {
        while (isInspecting)
        {
            inspectAudio.clip = inspectClip;
            inspectAudio.Play();

            yield return new WaitForSeconds(inspectAudio.clip.length);

            float wait = Random.Range(inspectMinDelay, inspectMaxDelay);
            yield return new WaitForSeconds(wait);
        }
    }
}


