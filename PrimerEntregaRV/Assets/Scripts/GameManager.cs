using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject pantallaGameOver;
    public GameObject jugador;

    void Awake()
    {
        instance = this;
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");

        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
            playerMove.enabled = false;

        PlayerInteraction playerInt = FindFirstObjectByType<PlayerInteraction>();
        if (playerInt != null)
            playerInt.enabled = false;

        if (pantallaGameOver != null)
            pantallaGameOver.SetActive(true);
    }

    public void Victoria()
    {
        Debug.Log("Secuencia correcta. La puerta se ha abierto.");
    }
}

