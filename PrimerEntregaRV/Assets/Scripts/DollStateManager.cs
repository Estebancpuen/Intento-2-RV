using UnityEngine;

public class DollStateManager : MonoBehaviour
{
    [Header("Partes de la muñeca en orden de pérdida")]
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftLeg;
    public GameObject rightLeg;

    private int mistakes = 0;
    private int maxMistakes = 4;

    public void RegisterMistake()
    {
        mistakes++;

        switch (mistakes)
        {
            case 1:
                RemoveLimb(leftArm);
                break;
            case 2:
                RemoveLimb(rightArm);
                break;
            case 3:
                RemoveLimb(leftLeg);
                break;
            case 4:
                RemoveLimb(rightLeg);
                TriggerGameOver();
                break;
        }
    }

    void RemoveLimb(GameObject limb)
    {
        if (limb == null) return;

        // Crear copia física
        GameObject droppedLimb = Instantiate(limb, limb.transform.position, limb.transform.rotation);

        // Quitar animaciones si tiene
        Animator anim = droppedLimb.GetComponent<Animator>();
        if (anim) Destroy(anim);

        Rigidbody rb = droppedLimb.AddComponent<Rigidbody>();
        rb.mass = 0.3f;

        // Desactivar la original (desaparece del cuerpo)
        limb.SetActive(false);

        Debug.Log("La muñeca perdió una extremidad...");
    }

    void TriggerGameOver()
    {
        Debug.Log("La muñeca está completamente destruida… GAME OVER");
        // Aquí luego ponemos pantalla de muerte, sonido, etc.
    }
}

