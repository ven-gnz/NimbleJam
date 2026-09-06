using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public enum SoundType
    {
        SFX, Music
    }

    [SerializeField] AudioClip FootStepA;
    [SerializeField] AudioClip FootStepB;
    [SerializeField] AudioClip Jump;
    [SerializeField] AudioClip Dig;
    [SerializeField] AudioClip Place;

    [SerializeField] AudioClip _music;

    private bool useFootStepA = true;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        Init();
    }

    [SerializeField] AudioMixer _mixer;
    AudioMixerGroup _musicGroup;
    AudioMixerGroup _sfxGroup;

    const string MUSIC_GROUP_NAME = "Music";
    const string SFX_GROUP_NAME = "SFX";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    const string MASTER_VOLUME_NAME = "MasterVolume";
    const string MUSIC_VOLUME_NAME = "MusicVolume";
    const string SFX_VOLUME_NAME = "SFXVolume";




  void Init()
    {
        _musicGroup = _mixer.FindMatchingGroups(MUSIC_GROUP_NAME)[0];
        _musicGroup = _mixer.FindMatchingGroups(SFX_GROUP_NAME)[0];
        PlayAudio(_music, SoundType.Music, 1.0f, true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }


    public void PlayAudio(AudioClip clip, SoundType type, float volume, bool loop)
    {
        GameObject newAudioSource = new(clip.name + "Source");
        AudioSource audioSource = newAudioSource.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();

        switch(type)
        {
            case SoundType.SFX:
                audioSource.outputAudioMixerGroup = Instance._sfxGroup;
                break;
            case SoundType.Music:
                audioSource.outputAudioMixerGroup = Instance._musicGroup;
                break;
        }

        if(!loop)
        {
            Destroy(audioSource.gameObject, clip.length);
        }
    }

    public void PlayJump()
    {
        PlayAudio(Jump, SoundType.SFX, 0.8f, false);
    }

    public void PlayDig()
    {
        PlayAudio(Dig, SoundType.SFX, 0.35f, false);
    }

    public void PlayPlace()
    {
        PlayAudio(Place, SoundType.SFX, 0.45f, false);
    }

    public void PlayFootStep()
    {
        AudioClip clip = useFootStepA ? FootStepA : FootStepB;
        useFootStepA = !useFootStepA;

        PlayAudio(clip, SoundType.SFX, 0.45f, false);
    }

    public void ChangeMasterVolume(float volume)
    {
        _mixer.SetFloat(MASTER_VOLUME_NAME, Mathf.Log10(volume) * 20);
    }
}
