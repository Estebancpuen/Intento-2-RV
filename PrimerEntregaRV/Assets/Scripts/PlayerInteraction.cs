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

        // Aumentamos la distancia de interacción a 5f por si el asiento está lejos
        if (Physics.Raycast(ray, out hit, 5f))
        {
            // PRIORIDAD: Detectar teclas
            PianoKey key = hit.collider.GetComponent<PianoKey>();
            if (key != null)
            {
                if (isSeated)
                {
                    PianoManager pm = Object.FindFirstObjectByType<PianoManager>();
                    if (pm != null && pm.dollIsSeated)
                    {
                        key.Press();
                    }
                    else
                    {
                        Debug.Log("No puedes tocar: la niña no está sentada correctamente.");
                    }
                }
                return; // Salir después de intentar tocar una tecla
            }

            // Detectar Agarrar Niña
            PickUpItem item = hit.collider.GetComponent<PickUpItem>();
            if (item != null && heldItem == null)
            {
                heldItem = item;
                item.OnPickup(holdPoint);
                return;
            }

            // Detectar Asiento
            if (hit.collider.CompareTag("AsientoJugador") && !isSeated)
            {
                SitDown(hit.collider.transform);
                return;
            }
        }
    }

    void SitDown(Transform seat)
    {
        isSeated = true;
        movementScript.canMove = false; // Mantenemos la rotación activa
        characterController.enabled = false;

        // Buscamos si el asiento tiene un hijo llamado "SitPoint"
        Transform customSitPoint = seat.Find("SitPoint");

        if (customSitPoint != null)
        {
            transform.position = customSitPoint.position;
            transform.rotation = customSitPoint.rotation;
        }
        else
        {
            // Si no existe el hijo, usamos el método anterior con un ajuste bajo
            transform.position = seat.position + Vector3.up * 0.1f;
            transform.rotation = seat.rotation;
        }

        // Lógica de la niña
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
