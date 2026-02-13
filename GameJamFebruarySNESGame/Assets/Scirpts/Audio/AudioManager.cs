using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioSource menuMusic;
    [SerializeField] AudioSource yellowMusic;
    [SerializeField] AudioSource mushroomMusic;
    [SerializeField] AudioSource swampMusic;
    [SerializeField] AudioSource iceMusic;
    [SerializeField] AudioSource creditsMusic;

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

    private AudioSource music;
    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume;

    private void Awake()
    {
        if (instance == null)           //GRRR singleton
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic(menuMusic);
                break;
            case "Reef":
                PlayMusic(yellowMusic);
                break;
            case "Town":
                PlayMusic(yellowMusic);
                break;
            case "Fungal":
                PlayMusic(mushroomMusic);
                break;
            case "Swamp":
                PlayMusic(swampMusic);
                break;
            case "Tundra":
                PlayMusic(iceMusic);
                break;
            case "Credits":
                PlayMusic(creditsMusic);
                break;
            default:
                PlayMusic(yellowMusic);
                break;
        }
    }

    private void PlayMusic(AudioSource audioSource)
    {
        if (music == audioSource) return;

        if (music != null) music.Stop();

        music = audioSource;
        music.volume = musicVolume;
        music.loop = true;
        music.Play();
    }

    public void PlayFireAudio()
    {
        playerSource.PlayOneShot(fireAudio, sfxVolume);
    }

    public void PlayIceAudio()
    {
        playerSource.PlayOneShot(iceAudio, sfxVolume);
    }

    public void PlayLightAudio()
    {
        playerSource.PlayOneShot(lightAudio, sfxVolume);
    }

    public void PlayTalkAudio()
    {
        playerSource.PlayOneShot(talkAudio, sfxVolume);
    }

    public void StopSFXAudio()
    {
        playerSource.Stop();
    }

    public void PlayPlantAudio()
    {
        playerSource.PlayOneShot(plantAudio, sfxVolume);
    }

    public void PlayHitAudio()
    {
        playerSource.PlayOneShot(hitAudio, sfxVolume);
    }

    public void PlayCollectAudio()
    {
        playerSource.PlayOneShot(collectAudio, sfxVolume);
    }
}
