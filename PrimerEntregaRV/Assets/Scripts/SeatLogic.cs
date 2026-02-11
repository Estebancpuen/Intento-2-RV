using UnityEngine;

public class SeatLogic : MonoBehaviour
{
    public PianoManager pianoManager;

   
    public void SitDoll()
    {
        pianoManager.dollIsSeated = true;
        pianoManager.dollNPC.transform.position = pianoManager.dollSeatPoint.position;
        pianoManager.dollNPC.transform.rotation = pianoManager.dollSeatPoint.rotation;

        PianoManager piano = FindFirstObjectByType<PianoManager>();
        if (piano != null)
        {
            piano.dollIsSeated = true;
            Debug.Log("La muñeca está sentada. Piano ACTIVADO.");
        }

        Debug.Log("La niña fue colocada en el asiento correctamente.");

        pianoManager.dollIsSeated = true;
        pianoManager.puzzleSolved = false; 
        pianoManager.OnPlayerSit();
    }
}
