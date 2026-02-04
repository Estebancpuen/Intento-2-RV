using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCam;
    public float interactionDistance = 5f;
    public Transform holdPoint;
    public GameObject interactUI;

    private PickUpItem heldItem;
    private bool isSeated = false;
    private PlayerMovement movementScript;
    private CharacterController characterController;

    [Header("UI Inspección")]
    public CanvasGroup inspectFadePanel;
    public float fadeSpeed = 5f;

    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
        if (interactUI) interactUI.SetActive(false);
    }

    void Update()
    {
        HandleRaycast();

        // Interacción principal con E
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteInteraction();
        }

        // Tocar con el click izquierdo si está sentado
        if (Input.GetMouseButtonDown(0) && isSeated)
        {
            TryTouchPiano();
        }

        HandleInspectFade();
    }

    void HandleRaycast()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Verificamos si el objeto es interactuable
            if (hit.collider.GetComponent<PianoKey>() ||
                hit.collider.GetComponent<PickUpItem>() ||
                hit.collider.CompareTag("AsientoJugador") ||
                hit.collider.CompareTag("AsientoMuñeca"))
            {
                hitSomething = true;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
        }

        if (interactUI) interactUI.SetActive(hitSomething);
    }

    void ExecuteInteraction()
    {

        if (heldItem != null && heldItem.isInspecting)
        {
            heldItem.OnPickup(holdPoint);
            movementScript.canMove = true;
            Debug.Log("Muñeca ahora está sostenida.");
            return;
        }


        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // 2. TECLAS DEL PIANO
            PianoKey key = hit.collider.GetComponent<PianoKey>();
            if (key != null)
            {
                key.Press();
                return;
            }

            // 3. AGARRAR / INSPECCIONAR (Solo si no tenemos nada)
            PickUpItem item = hit.collider.GetComponent<PickUpItem>();
            if (item != null && heldItem == null && !isSeated)
            {
                heldItem = item;
                item.OnInspect(playerCam.transform);
                movementScript.canMove = false;
                return;
            }

            // 4. SENTAR A LA MUÑECA (Solo si ya la tenemos en la mano)
            if (isSeated && heldItem != null && heldItem.isHeld && hit.collider.CompareTag("AsientoMuñeca"))
            {
                PlaceDollOnSeat(hit.collider.gameObject);
                return;
            }

            // 5. SENTARSE / LEVANTARSE EL JUGADOR
            if (hit.collider.CompareTag("AsientoJugador") && !isSeated)
            {
                SitDown(hit.collider.transform);
                return;
            }
        }

        // 6. LEVANTARSE
        if (isSeated && (heldItem == null || !heldItem.isInspecting))
        {
            StandUp();
            return;
        }
    }

    // Función auxiliar para el click del ratón
    void TryTouchPiano()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            PianoKey key = hit.collider.GetComponent<PianoKey>();
            if (key != null) key.Press();
        }
    }

    void SitDown(Transform seat)
    {
        isSeated = true;
        movementScript.canMove = false;
        if (characterController) characterController.enabled = false;

        Transform customSitPoint = seat.Find("SitPoint");
        transform.position = customSitPoint ? customSitPoint.position : seat.position + Vector3.up * 0.1f;
        transform.rotation = customSitPoint ? customSitPoint.rotation : seat.rotation;
    }

    void PlaceDollOnSeat(GameObject seatObject)
    {
        SeatLogic seatLogic = seatObject.GetComponentInParent<SeatLogic>();
        if (seatLogic != null)
        {
            heldItem.OnDrop();
            heldItem.transform.position = seatLogic.pianoManager.dollSeatPoint.position;
            heldItem.transform.rotation = seatLogic.pianoManager.dollSeatPoint.rotation;
            seatLogic.SitDoll();
            heldItem = null;
        }

    }

    void StandUp()
    {
        isSeated = false;
        if (characterController) characterController.enabled = true;
        movementScript.canMove = true;
    }

    public void ResetHeldItem()
    {
        if (heldItem != null)
        {
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;

            heldItem.transform.SetParent(null);
            heldItem = null;
        }
    }

    public bool IsInspecting()
    {
        return heldItem != null && heldItem.isInspecting;
    }

   

    void HandleInspectFade()
    {
        if (inspectFadePanel == null) return;

        bool inspecting = heldItem != null && heldItem.isInspecting;
        float targetAlpha = inspecting ? 1f : 0f;

        inspectFadePanel.alpha = Mathf.MoveTowards(
            inspectFadePanel.alpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        inspectFadePanel.blocksRaycasts = inspecting;
        inspectFadePanel.interactable = inspecting;
    }

}
