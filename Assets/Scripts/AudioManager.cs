using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    // Singleton
    public static AudioManager instance;

    // Volume
    public float volume = 1f;

    // Audio Mixer Groups
    [SerializeField] private AudioMixerGroup UIAudioMixerGroup;
    [SerializeField] private AudioMixerGroup GameMusicMixer;

    // Music
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip menuMusic;

    // Sound Affects Game
    [SerializeField] private AudioClip pistolFire;
    [SerializeField] private AudioClip pistolReload;
    [SerializeField] private AudioClip machineGunFire;
    [SerializeField] private AudioClip machineGunReload;
    [SerializeField] private AudioClip shotgunFire;
    [SerializeField] private AudioClip shotgunReload;

    // Sound Affects UI
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip shopPurchase;

    [SerializeField] private int alertedEnemiesCount = 0;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (alertedEnemiesCount > 0)
        {
            SetGameMusicPitch(1.5f, 1.5f);
        }
        else
        {
            SetGameMusicPitch(1f);
        }
    }

    public void IsEnemyAlerted(bool isAlerted)
    {
        if (isAlerted)
        {
            alertedEnemiesCount++;
        }
        else
        {
            alertedEnemiesCount--;
        }

        if (alertedEnemiesCount <= 0)
        {
            alertedEnemiesCount = 0;
        }
    }

    public void ResetEnemyCount()
    {
        alertedEnemiesCount = 0;
    }

    public void PlaySFX(string sfx, Vector3 audioPosition, float sfxVolume = 1f)
    {
        AudioClip clip = null;

        switch (sfx)
        {
            case "pistolFire":
                clip = pistolFire;
                break;
            case "machineGunFire":
                clip = machineGunFire;
                break;
            case "shotgunFire":
                clip = shotgunFire;
                break;
            case "buttonClick":
                clip = buttonClick;
                break;
            case "shopPurchase":
                clip = shopPurchase;
                break;
            
        }

        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, audioPosition, volume * sfxVolume);
        }
    }

    public void Play2DSFX(string sfx, float sfxVolume = 1f, bool needsDecibleIncrease = false)
    {
        AudioClip clip = null;

        switch (sfx)
        {
            case "pistolReload":
                clip = pistolReload;
                break;
            case "machineGunReload":
                clip = machineGunReload;
                break;
            case "shotgunReload":
                clip = shotgunReload;
                break;
            case "buttonClick":
                clip = buttonClick;
                break;
            case "shopPurchase":
                clip = shopPurchase;
                break;
        }

        if (clip != null)
        {
            GameObject tempAudio = new GameObject("Temp2DAudio");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = clip;
            if (needsDecibleIncrease) // Uses AudioMixer to increase decibels
            {
                audioSource.outputAudioMixerGroup = UIAudioMixerGroup;
            }
            audioSource.volume = sfxVolume * volume;
            audioSource.spatialBlend = 0f; 
            audioSource.Play();

            Destroy(tempAudio, clip.length);
        }
    }

    public void SetGameMusicPitch(float pitch, float volume = 0.0f)
    {
        GameMusicMixer.audioMixer.SetFloat("Pitch", pitch);
        GameMusicMixer.audioMixer.SetFloat("Volume", volume);
    }

    public void PlayMusic(string music)
    {
        AudioClip clip = null;
        AudioSource audioSource = null;

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        switch (music)
        {
            case "gameMusic":
                clip = gameMusic;
                audioSource.outputAudioMixerGroup = GameMusicMixer;
                audioSource.volume = 0.6f;
                ResetEnemyCount();
                break;
            case "menuMusic":
                clip = menuMusic;
                audioSource.outputAudioMixerGroup = null;
                audioSource.volume = 0.3f;
                break;
        }


        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.Play();
    }

    public void StopMusic()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    
}
