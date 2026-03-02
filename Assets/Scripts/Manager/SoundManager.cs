using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<Sound> sounds;

    private Dictionary<SoundType, AudioClip> soundDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        soundDict = new Dictionary<SoundType, AudioClip>();

        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.type))
                soundDict.Add(s.type, s.clip);
        }
    }

    public void PlaySFX(SoundType type)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("SFX Source not assigned!");
            return;
        }

        if (soundDict.TryGetValue(type, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlaySFX(SoundType type, float volume = 1f)
    {
        if (soundDict.TryGetValue(type, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}

public enum SoundType
{
    Select,
    Deselect,
    BottlePour,
    BottleFull,
    Win,
    BottleClose
}

[System.Serializable]
public class Sound
{
    public SoundType type;
    public AudioClip clip;
}