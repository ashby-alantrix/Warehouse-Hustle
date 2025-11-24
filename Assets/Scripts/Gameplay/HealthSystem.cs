using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    [SerializeField] private int maxLifes = 5;

    private int availableLifes = 5;
    private double totalSeconds;
    private double totalTimeOffInSeconds, lastElapsedSeconds;
    private bool startHealthTimer = false;

    private UserDataBehaviour userDataBehaviour;
    private PopupManager popupManager;
    private HealthData gameHealthData;
    private UserHealthData userHealthData;
    private GetMoreLivesPopup getMoreLivesPopup;
    private TimeData timeData;

    public bool IsFull => availableLifes == maxLifes;

    public int AvailableLifes => availableLifes;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<HealthSystem>(this);
    }

    public void InitializeData()
    {
        userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
        getMoreLivesPopup = popupManager.GetScreen<GetMoreLivesPopup>(UIType.GetMoreLivesPopup);

        gameHealthData = userDataBehaviour.GetHealthData();
        userHealthData = userDataBehaviour.GetUserHealthData();

        totalSeconds = gameHealthData.timeInSecondsForOneLife;
        availableLifes = userDataBehaviour.IsFirstUserSession() ? gameHealthData.totalLifes : userHealthData.attainedLifes;

        if (!userDataBehaviour.IsFirstUserSession())
        {
            timeData = userDataBehaviour.GetTimeData();
            DateTime savedTime = DateTime.Parse(timeData.lastSavedProgressTime);
            DateTime currentTime = DateTime.UtcNow;

            TimeSpan timeDiff = currentTime - savedTime;
            totalTimeOffInSeconds =  timeDiff.TotalSeconds;
            lastElapsedSeconds = int.Parse(timeData.lastElapsedSeconds);

            if (totalTimeOffInSeconds > lastElapsedSeconds)
            {
                totalTimeOffInSeconds -= lastElapsedSeconds;
                // do it until health is filled with the totalTimeOffInSeconds
            }
            else
            {
                lastElapsedSeconds -= totalTimeOffInSeconds;
                totalSeconds = lastElapsedSeconds;
                startHealthTimer = true;
                // continue the timer logic
            }
        }
    }

    private void Update()
    {
        if (!startHealthTimer) 
        {
            return;
        }

        if (totalSeconds > 0)
        {
            totalSeconds -= Time.deltaTime;
        }
        else
        {
            startHealthTimer = false;
            UpdateAvailableLives(1); 
        }
    }

    public string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"mm\:ss");
    }

    public void UpdateAvailableLives(int life)
    {
        availableLifes += life;
        getMoreLivesPopup.UpdateAvailableLifes(availableLifes);

        if (!IsFull)
        {
            totalSeconds = gameHealthData.timeInSecondsForOneLife;
        }
        else
        {
            startHealthTimer = false;
        }
    }

    private void SetLastProgressTime()
    {
        string timeString = DateTime.UtcNow.ToString("o"); // ISO 8601
        userDataBehaviour.SetLastProgressTime(timeString, $"{totalSeconds}");
    }

    private void OnApplicationFocus(bool focus)
    {
        SetLastProgressTime();
    }

    private void OnApplicationQuit()
    {
        SetLastProgressTime();
    }
}
