using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Truck_Next_Point,
    Truck_Dest_Point
}

[System.Serializable]
public class SoundData
{
    public SoundType soundType;
    public int priority;
    public AudioClip soundClip;
}

public class SoundManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    [SerializeField] AudioSource primaryAudioSource;
    [SerializeField] private SoundData[] soundDatas;

    private float volTimer = 0, cachedSecVolTimer = 0;
    private bool startVolTimer = false;

    private Dictionary<SoundType, SoundData> soundDataDict = new Dictionary<SoundType, SoundData>();
    private Dictionary<SoundType, AudioSource> secondaryAudioSourcesDict = new Dictionary<SoundType, AudioSource>();

    private AudioSource secondaryAudioSource;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<SoundManager>(this);
    }

    public void InitializeData()
    {
        primaryAudioSource.priority = 0;
        for (int idx = 0; idx < soundDatas.Length; idx++)
        {
            if (soundDataDict.ContainsKey(soundDatas[idx].soundType))
                soundDataDict[soundDatas[idx].soundType] =  soundDatas[idx];
            else 
                soundDataDict.Add(soundDatas[idx].soundType, soundDatas[idx]);
        }
    }

    public void RegisterAudioSource(SoundType soundType, AudioSource audioSource)
    {
        if (!secondaryAudioSourcesDict.ContainsKey(soundType))
            secondaryAudioSourcesDict.Add(soundType, audioSource);
        else
        {
            secondaryAudioSourcesDict[soundType] = audioSource;
        }
    }

    public void PlayPrimarySoundClip(SoundType soundType)
    {
        return;
        if (!enabled) return;

        SoundData soundData = soundDataDict[soundType];

        primaryAudioSource.priority = soundData.priority;
        primaryAudioSource.PlayOneShot(soundData.soundClip);
    }

    public void PlaySecondarySoundClip(SoundType soundType)
    {
        if (!enabled) return;

        SoundData soundData = soundDataDict[soundType];
        
        secondaryAudioSource = secondaryAudioSourcesDict[soundType];
        Debug.Log($"secondaryAudioSource. volume: {secondaryAudioSource.volume}");
        secondaryAudioSource.priority = soundData.priority;
        secondaryAudioSource.PlayOneShot(soundData.soundClip);
        cachedSecVolTimer = volTimer = secondaryAudioSource.volume;
        startVolTimer = true;
    }

    void Update()
    {
        // Debug.Log($"")
        if (!startVolTimer) return;

        if (volTimer > 0)
        {
            volTimer -= Time.deltaTime/4;
            Debug.Log($"secondaryAudioSource. Reducing timer vol");
            secondaryAudioSource.volume = volTimer;
        }
        else
        {
            volTimer = 0;
            startVolTimer = false;
            secondaryAudioSource.volume = cachedSecVolTimer;
            secondaryAudioSource.Stop();
        }
    }
}
