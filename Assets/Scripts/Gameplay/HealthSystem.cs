using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    [SerializeField] private int maxLifes = 5;

    private int availableLifes = 5;
    private double totalSecondsRem;
    private double totalTimeOffInSeconds, prevTimeInSecondsRem;
    private bool startHealthTimer = false, hasTimerFilled = false;

    private UserDataBehaviour userDataBehaviour;
    private PopupManager popupManager;
    private HealthData gameHealthData;
    private UserHealthData userHealthData;
    private GetMoreLivesPopup getMoreLivesPopup;
    private TimeData timeData;

    public bool IsFull => availableLifes == gameHealthData.totalLifes;
    public HealthData GameHealthData => gameHealthData;

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

        totalSecondsRem = gameHealthData.timeInSecondsForOneLife;
        availableLifes = userDataBehaviour.IsFirstUserSession() ? gameHealthData.totalLifes : userHealthData.attainedLifes;

        getMoreLivesPopup.SetHealthContent(IsFull);

        Debug.Log($"time :: availableLifes: {availableLifes}");

        if (!userDataBehaviour.IsFirstUserSession())
        {
            timeData = userDataBehaviour.GetTimeData();
            DateTime savedTime = DateTime.Parse(timeData.lastPlayedProgressTime);
            Debug.Log($"time :: timeData.lastPlayedProgressTime: {timeData.lastPlayedProgressTime}");
            Debug.Log($"time :: savedTime: {savedTime}");
            DateTime currentTime = DateTime.UtcNow;

            TimeSpan timeDiff = currentTime - savedTime;
            totalTimeOffInSeconds = timeDiff.TotalSeconds;
            Debug.Log($"time :: totalTimeOffInSeconds: {totalTimeOffInSeconds}");
            Debug.Log($"time :: timeData.lastElapsedSeconds: {timeData.lastElapsedSeconds}");
            prevTimeInSecondsRem = Double.Parse(timeData.lastElapsedSeconds);
            
            Debug.Log($"time :: prevTimeInSecondsRem: {prevTimeInSecondsRem}");

            if (!IsFull)
                UpdateBasedOnSavedTime();
        }
    }

    private void UpdateBasedOnSavedTime()
    {
        var trackedSeconds = prevTimeInSecondsRem == 0 ? gameHealthData.timeInSecondsForOneLife : prevTimeInSecondsRem;
        if (totalTimeOffInSeconds > trackedSeconds)
        {
            totalTimeOffInSeconds -= trackedSeconds;
            prevTimeInSecondsRem = 0;
            UpdateAvailableLives(1);
            // do it until health is filled with the totalTimeOffInSeconds
        }
        else // if trackedSeconds >= totalTimeOffInSeconds
        {
            trackedSeconds -= totalTimeOffInSeconds;
            totalTimeOffInSeconds = 0;
            totalSecondsRem = trackedSeconds;
            startHealthTimer = true;
        }
    }

    private void Update()
    {
        if (!startHealthTimer) 
        {
            return;
        }

        if (totalSecondsRem > 0)
        {
            totalSecondsRem -= Time.deltaTime;
        }
        else
        {
            startHealthTimer = false;
            if (!IsFull)
            {
                hasTimerFilled = true;
                UpdateAvailableLives(1); 
            }
        }
    }

    public string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSecondsRem);
        Debug.Log($"time :: GetFormattedTime() :: {time.ToString(@"mm\:ss")}");
        return time.ToString(@"mm\:ss");
    }

    public void UpdateAvailableLives(int life)
    {
        // on Replay clicked in Restart popup
        // if (availableLifes == 1 && Mathf.Sign(life) < 0)
        // {

        //     return;
        // }

        availableLifes += life;
        if (IsFull)
        {
            startHealthTimer = false;
            getMoreLivesPopup?.SetHealthContent(true);
            return;
        }

        if (getMoreLivesPopup && !getMoreLivesPopup.IsLifeToFillContentActive)
            getMoreLivesPopup.SetHealthContent(false);

        if (getMoreLivesPopup != null && getMoreLivesPopup.gameObject.activeInHierarchy)
            getMoreLivesPopup.UpdateAvailableLifes(availableLifes);

        if (totalTimeOffInSeconds > 0)
            UpdateBasedOnSavedTime();
        else 
        {
            totalSecondsRem = hasTimerFilled ? gameHealthData.timeInSecondsForOneLife : totalSecondsRem;
            hasTimerFilled = false;
            startHealthTimer = true;
        }
    }

    private void SetLastProgressTime()
    {
        if (gameHealthData != null && IsFull) return;
        
        if (userDataBehaviour)
        {
            userDataBehaviour.SetLastProgressTime($"{DateTime.UtcNow}", $"{totalSecondsRem}");
        }
    }

    private void SetUserHealthData()
    {
        if (userDataBehaviour)
        {
            userHealthData.attainedLifes = availableLifes;
            userDataBehaviour.SetUserHealthData(userHealthData);
        }
    }

    private void SetDatasForSaving()
    {
        SetUserHealthData();
        SetLastProgressTime();
    }

    private void OnDestroy()
    {
        Debug.Log($"ExitCallback {name} OnDestroy");
        SetDatasForSaving();
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log($"ExitCallback OnApplicationFocus");
        SetDatasForSaving();
    }

    private void OnApplicationQuit()
    {
        Debug.Log($"ExitCallback OnApplicationQuit");
        SetDatasForSaving();
    }
}
