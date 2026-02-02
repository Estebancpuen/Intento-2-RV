using UnityEngine;

public class SeatLogic : MonoBehaviour
{
    public PianoManager pianoManager;
    public float detectionRadius = 1.5f;

    void Update()
    {
        // Solo verificar distancia si el manager dice que NO está sentada aún
        if (pianoManager.dollNPC != null && !pianoManager.dollIsSeated)
        {
            float distance = Vector3.Distance(transform.position, pianoManager.dollNPC.transform.position);
            if (distance < detectionRadius)
            {
                SitDoll();
            }
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
