using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCam;
    public float interactionDistance = 3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo para tocar
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Buscamos si el objeto que tocamos tiene el script de la tecla
            PianoKey key = hit.collider.GetComponent<PianoKey>();
            if (key != null)
            {
                key.Press();
                // Aquí avisaremos al sistema de puzzles más tarde
            }
        }
    }
}
