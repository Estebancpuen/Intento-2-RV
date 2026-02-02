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

        // PRIORIDAD: Si estamos inspeccionando, el primer clic/E solo sirve para dejar de inspeccionar
        if (heldItem != null && heldItem.isInspecting)
        {
            heldItem.OnPickup(holdPoint);
            movementScript.canMove = true;
            return; // Salimos inmediatamente para no detectar el asiento en este frame
        }


        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 1. SI ESTÁ INSPECCIONANDO: Al presionar E, solo debe pasar a la mano.
        if (heldItem != null && heldItem.isInspecting)
        {
            heldItem.OnPickup(holdPoint);
            movementScript.canMove = true;
            return; // IMPORTANTE: Salimos de la función aquí para que no haga nada más este frame
        }

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
        if (isSeated && !isSeatedConMuñecaEnMano()) // Función opcional para evitar levantarse por error
        {
            StandUp();
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
        heldItem = null;
    }

    // Función para verificar si el jugador está sentado y tiene a la muñeca
    bool isSeatedConMuñecaEnMano()
    {
        return isSeated && heldItem != null;
    }
}
