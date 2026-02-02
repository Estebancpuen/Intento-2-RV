using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isHeld = false;
    private Transform holdParent;

    public void OnPickup(Transform parent)
    {
        isHeld = true;
        holdParent = parent;

        // Desactivar físicas para que no se caiga mientras la llevas
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = true;

        // Hacerla hija del punto de agarre del jugador
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDrop()
    {
        isHeld = false;
        transform.SetParent(null);
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = false;
    }
}
