using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCam;
    public float interactionDistance = 3f;
    public Transform holdPoint; // Arrastra el objeto "HoldPoint" aquí

    private PickUpItem heldItem;
    private bool isSeated = false;
    private Vector3 originalPosition;
    private PlayerMovement movementScript; // Tu script de caminar
    private CharacterController characterController; // DECLARACIÓN FALTANTE

    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>(); // ASIGNACIÓN
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Usaremos la E para interactuar
        {
            if (isSeated) { StandUp(); }
            else { ShootRay(); }
        }
    }

    void ShootRay()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // 1. LÓGICA DE AGARRAR NIÑA
            PickUpItem item = hit.collider.GetComponent<PickUpItem>();
            if (item != null && heldItem == null)
            {
                heldItem = item;
                item.OnPickup(holdPoint);
                return;
            }

            // 2. LÓGICA DE SENTARSE (Si toca el cilindro grande)
            if (hit.collider.CompareTag("AsientoJugador"))
            {
                SitDown(hit.collider.transform);
                return;
            }

            // 3. LÓGICA DE TECLAS (Si ya está sentado)
            if (isSeated)
            {
                PianoKey key = hit.collider.GetComponent<PianoKey>();
                if (key != null)
                {
                    // Solo intentamos tocar si la niña está sentada
                    PianoManager pm = FindObjectOfType<PianoManager>();
                    if (pm != null && pm.dollIsSeated)
                    {
                        key.Press();
                    }
                    else
                    {
                        Debug.Log("No puedes tocar el piano solo... necesitas compañía.");
                    }
                }
            }
        }
    }

    void SitDown(Transform seat)
    {
        isSeated = true;
        movementScript.canMove = false; // Solo bloqueamos el caminar, no la rotación
        characterController.enabled = false;

        transform.position = seat.position + Vector3.up * 0.5f;
        transform.rotation = seat.rotation;

        if (heldItem != null)
        {
            SeatLogic seatLogic = FindObjectOfType<SeatLogic>();
            if (seatLogic != null)
            {
                heldItem.OnDrop();
                heldItem = null;
                seatLogic.SitDoll();
            }
        }
    }

    void StandUp()
    {
        isSeated = false;
        characterController.enabled = true;
        movementScript.canMove = true; // Devolvemos el control del caminar
    }
}
