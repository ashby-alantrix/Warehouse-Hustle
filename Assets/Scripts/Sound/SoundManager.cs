using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Truck_Next_Point,
    Truck_Dest_Point,
    Level_Lost,
    Level_Win,
    Node_Click,
    Node_Filled,
    Swap,
    Button_Click
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
    [SerializeField] AudioSource audioSource;
    [SerializeField] private SoundData[] soundDatas;

    private float volTimer = 0, cachedSecVolTimer = 0;
    private bool startVolTimer = false;
    public bool IsGameSoundOn
    {
        get;
        private set;
    }

    private Dictionary<SoundType, SoundData> soundDataDict = new Dictionary<SoundType, SoundData>();
    private Dictionary<SoundType, AudioSource> secondaryAudioSourcesDict = new Dictionary<SoundType, AudioSource>();

    private AudioSource secondaryAudioSource;

    private UserDataBehaviour userDataBehaviour;
    private InGameSoundData soundData;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<SoundManager>(this);
    }

    public void InitializeData()
    {
        userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();

        audioSource.priority = 0;
        for (int idx = 0; idx < soundDatas.Length; idx++)
        {
            if (soundDataDict.ContainsKey(soundDatas[idx].soundType))
                soundDataDict[soundDatas[idx].soundType] =  soundDatas[idx];
            else 
                soundDataDict.Add(soundDatas[idx].soundType, soundDatas[idx]);
        }

        soundData = userDataBehaviour.GetSoundData();
        IsGameSoundOn = soundData.gameSoundToggle;
    }

    public void SetGameSound(bool state)
    {
        IsGameSoundOn = state;
        soundData.gameSoundToggle = state;
        userDataBehaviour.SaveSoundData(soundData);
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

    public void PlayPrimaryGameSoundClip(SoundType soundType)
    {
        if (!enabled || !IsGameSoundOn) return;

        SoundData soundData = soundDataDict[soundType];

        audioSource.priority = soundData.priority;
        audioSource.PlayOneShot(soundData.soundClip);
    }

    public void PlayButtonSoundClip(SoundType soundType)
    {
        SoundData soundData = soundDataDict[soundType];

        audioSource.priority = soundData.priority;
        audioSource.PlayOneShot(soundData.soundClip);
    }

    public void PlaySecondaryGameSoundClip(SoundType soundType)
    {
        if (!enabled || !IsGameSoundOn) return;

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
