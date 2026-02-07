using UnityEngine;

public class DollStateManager : MonoBehaviour
{
    
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftLeg;
    public GameObject rightLeg;

    private int mistakes = 0;
    //private int maxMistakes = 4;

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
                GameManager.instance.GameOver();
                break;
        }
    }

    void RemoveLimb(GameObject limb)
    {
        if (limb == null) return;

        
        GameObject droppedLimb = Instantiate(limb, limb.transform.position, limb.transform.rotation);

       
        Animator anim = droppedLimb.GetComponent<Animator>();
        if (anim) Destroy(anim);

        Rigidbody rb = droppedLimb.AddComponent<Rigidbody>();
        rb.mass = 0.3f;

        
        limb.SetActive(false);

        Debug.Log("La muñeca perdió una extremidad...");
    }

    void TriggerGameOver()
    {
        Debug.Log("La muñeca está completamente destruida… GAME OVER");
        
    }
}

