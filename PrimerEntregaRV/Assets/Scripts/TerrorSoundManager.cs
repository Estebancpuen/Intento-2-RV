using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrorSoundDirector : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Sonidos")]
    public List<AudioClip> passingSounds;
    public List<AudioClip> behindSounds;

    [Header("Tiempo")]
    public float minDelay = 5f;
    public float maxDelay = 12f;

    [Header("Movimiento")]
    public float speed = 18f;
    public float sideOffset = 3f;
    public float forwardOffset = 6f;
    public float behindDistance = 4f;

    [Header("Audio")]
    public float minVolume = 0.25f;
    public float maxVolume = 1f;
    public float maxDistance = 15f;
    public float doppler = 1.2f;

    private AudioSource source;
    private Vector3 direction;

    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.maxDistance = maxDistance;
        source.dopplerLevel = doppler;
        source.loop = false;

        StartCoroutine(SoundRoutine());
    }

    IEnumerator SoundRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            bool playBehind =
                behindSounds.Count > 0 &&
                (passingSounds.Count == 0 || Random.value > 0.5f);

            if (playBehind)
                PlayBehindSound();
            else if (passingSounds.Count > 0)
                PlayPassingSound();

            while (source.isPlaying)
            {
                UpdateMovement();
                yield return null;
            }
        }
    }

    // 🔥 Sonidos que cruzan al jugador
    void PlayPassingSound()
    {
        source.clip = passingSounds[Random.Range(0, passingSounds.Count)];

        Vector3 side = Random.value > 0.5f ? player.right : -player.right;

        transform.position =
            player.position
            - player.forward * forwardOffset
            + side * sideOffset;

        Vector3 target =
            player.position
            + player.forward * forwardOffset
            - side * sideOffset;

        direction = (target - transform.position).normalized;

        source.volume = minVolume;
        source.Play();
    }

    // 👁️ Sonidos SOLO detrás del jugador
    void PlayBehindSound()
    {
        source.clip = behindSounds[Random.Range(0, behindSounds.Count)];

        transform.position =
            player.position
            - player.forward * behindDistance
            + Vector3.up * Random.Range(-0.5f, 0.5f);

        direction = Vector3.zero; // No se mueve

        source.volume = maxVolume * 0.8f;
        source.Play();
    }

    void UpdateMovement()
    {
        if (direction != Vector3.zero)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        float t = Mathf.InverseLerp(maxDistance, 1.2f, distance);
        source.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }
}


