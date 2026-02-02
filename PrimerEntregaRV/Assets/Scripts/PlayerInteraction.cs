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

    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
        if (interactUI) interactUI.SetActive(false);
    }

    void Update()
    {
        HandleRaycast();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isSeated) StandUp();
            else ExecuteInteraction();
        }

        // Nueva opción: Tocar con el click
        if (Input.GetMouseButtonDown(0) && isSeated)
        {
            ExecuteInteraction();
        }
    }

    void HandleRaycast()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.GetComponent<PianoKey>() ||
                hit.collider.GetComponent<PickUpItem>() ||
                hit.collider.CompareTag("AsientoJugador"))
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
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // 1. PRIORIDAD: Si miramos una tecla, la tocamos (estemos sentados o no)
            PianoKey key = hit.collider.GetComponent<PianoKey>();
            if (key != null)
            {
                key.Press();
                return; // Importante: Salimos para no ejecutar StandUp()
            }

            // 2. RECOGER MUÑECA
            PickUpItem item = hit.collider.GetComponent<PickUpItem>();
            if (item != null && heldItem == null && !isSeated)
            {
                heldItem = item;
                item.OnPickup(holdPoint);
                return;
            }

            // 3. SENTARSE (Solo si no estamos sentados ya)
            if (hit.collider.CompareTag("AsientoJugador") && !isSeated)
            {
                SitDown(hit.collider.transform);
                return;
            }
        }

        // 4. SI NO MIRAMOS NADA INTERACTUABLE Y ESTAMOS SENTADOS, NOS LEVANTAMOS
        if (isSeated)
        {
            StandUp();
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

        if (heldItem != null)
        {
            SeatLogic seatLogic = Object.FindFirstObjectByType<SeatLogic>();
            if (seatLogic != null)
            {
                heldItem.OnDrop();
                heldItem.transform.position = seatLogic.pianoManager.dollSeatPoint.position;
                heldItem.transform.rotation = seatLogic.pianoManager.dollSeatPoint.rotation;
                seatLogic.SitDoll();
                heldItem = null;
            }
        }
    }

    void StandUp()
    {
        isSeated = false;
        if (characterController) characterController.enabled = true;
        movementScript.canMove = true;
    }
}
