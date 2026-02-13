using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioSource gameMusic;

    [Header("Playersounds")]
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioClip fireAudio;
    [SerializeField] AudioClip iceAudio;
    [SerializeField] AudioClip lightAudio;
    [SerializeField] AudioClip talkAudio;
    [SerializeField] AudioClip plantAudio;
    [SerializeField] AudioClip hitAudio;
    [SerializeField] AudioClip collectAudio;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)           //GRRR singleton
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGameMusic()
    {
        gameMusic.Play();
        gameMusic.volume = 1.0f;
    }

    public void StopGameMusic()
    {
        gameMusic.Stop();
    }

    public void PlayFireAudio()
    {
        playerSource.PlayOneShot(fireAudio);
    }

    public void PlayIceAudio()
    {
        playerSource.PlayOneShot(iceAudio);
    }

    public void PlayLightAudio()
    {
        playerSource.PlayOneShot(lightAudio);
    }

    public void PlayTalkAudio()
    {
        playerSource.PlayOneShot(talkAudio);
    }

    public void PlayPlantAudio()
    {
        playerSource.PlayOneShot(plantAudio);
    }

    public void PlayHitAudio()
    {
        playerSource.PlayOneShot(hitAudio);
    }

    public void PlayCollectAudio()
    {
        playerSource.PlayOneShot(collectAudio);
    }
}
