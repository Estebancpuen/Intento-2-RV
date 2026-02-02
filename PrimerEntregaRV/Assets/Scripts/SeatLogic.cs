using UnityEngine;

public class SeatLogic : MonoBehaviour
{
    public PianoManager pianoManager;

    // Ahora SOLO se llama manualmente
    public void SitDoll()
    {
        pianoManager.dollIsSeated = true;
        pianoManager.dollNPC.transform.position = pianoManager.dollSeatPoint.position;
        pianoManager.dollNPC.transform.rotation = pianoManager.dollSeatPoint.rotation;

        Debug.Log("La niña fue colocada en el asiento correctamente.");
    }
}
