using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public CharacterController controller;
    public PlayerMovement movement;

    void Update()
    {
        if (audioSource == null || controller == null || movement == null)
            return;

        if (!controller.isGrounded || !movement.canMove)
        {
            StopSteps();
            return;
        }

        bool isMoving =
            Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f ||
            Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;

        if (isMoving)
        {
            PlaySteps();
        }
        else
        {
            StopSteps();
        }
    }

    void PlaySteps()
    {
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }

    void StopSteps()
    {
        if (!audioSource.isPlaying) return;
        audioSource.Stop();
    }
}



