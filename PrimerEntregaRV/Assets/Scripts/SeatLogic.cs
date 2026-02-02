using UnityEngine;

public class SeatLogic : MonoBehaviour
{
    public PianoManager pianoManager;
    public float detectionRadius = 1.5f;

    void Update()
    {
        // Si el jugador suelta a la niña cerca del asiento (puedes llamar a esto desde tu script de agarrar)
        // Por ahora, lo haremos automático si la niña está cerca
        float distance = Vector3.Distance(transform.position, pianoManager.dollNPC.transform.position);

        if (distance < detectionRadius && !pianoManager.dollIsSeated)
        {
            SitDoll();
        }
    }

    public void SitDoll()
    {
        pianoManager.dollIsSeated = true;
        // Acomodamos a la niña exactamente en el asiento
        pianoManager.dollNPC.transform.position = pianoManager.dollSeatPoint.position;
        pianoManager.dollNPC.transform.rotation = pianoManager.dollSeatPoint.rotation;
        Debug.Log("La niña se siente a esperar la música...");
    }
}
