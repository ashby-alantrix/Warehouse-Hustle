using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    [SerializeField] private int maxLifes = 5;
    [SerializeField] private bool enableTimer = true;

    private int availableLifes = 5;
    private double totalSecondsRem;
    private double totalTimeOffInSeconds, prevTimeInSecondsRem;
    private bool startHealthTimer = false, hasTimerFilled = false;

    private UserDataBehaviour userDataBehaviour;
    private PopupManager popupManager;
    private ScreenManager screenManager;
    private HealthData gameHealthData;
    private UserHealthData userHealthData;
    private GetMoreLivesPopup getMoreLivesPopup;
    private GlobalHUDScreen globalHUDScreen;
    private TimeData timeData;

    public bool IsFull => availableLifes == gameHealthData.totalLifes;
    public bool HaveHealthLeft => availableLifes > 0;
    public HealthData GameHealthData => gameHealthData;
    public UserHealthData UserHealthData => userHealthData;

    public int AvailableLifes => availableLifes;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<HealthSystem>(this);

        #if !UNITY_EDITOR
            enableTimer = true;
        #endif
    }

    public void SetFreeRefillState(bool isFreeRefill)
    {
        userHealthData.haveUsedFreeRefill = isFreeRefill;
        SetUserHealthData();
    }

    public void InitializeData()
    {
        Debug.Log($"HealthSystem: InitializeData");

        userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
        screenManager = InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>();

        getMoreLivesPopup = popupManager.GetPopup<GetMoreLivesPopup>(PopupType.GetMoreLivesPopup);
        globalHUDScreen = screenManager.GetScreen<GlobalHUDScreen>(ScreenType.GlobalHUDScreen);

        gameHealthData = userDataBehaviour.GetHealthData();
        userHealthData = userDataBehaviour.GetUserHealthData();

        totalSecondsRem = gameHealthData.timeInSecondsForOneLife;
        availableLifes = userDataBehaviour.IsFirstUserSession() ? gameHealthData.totalLifes : userHealthData.attainedLifes;

        getMoreLivesPopup.SetHealthContent(IsFull);
        globalHUDScreen.UpdateLivesButtonContainer(IsFull);

        Debug.Log($"hasenoughhealth :: availableLifes: {availableLifes}");

        timeData = userDataBehaviour.GetTimeData();
        if (!userDataBehaviour.IsFirstUserSession() && timeData != null)
        {
            if (!string.IsNullOrWhiteSpace(timeData.lastPlayedProgressTime) && !IsFull)
            {
                DateTime savedTime = DateTime.Parse(timeData.lastPlayedProgressTime);
                Debug.Log($"time :: timeData.lastPlayedProgressTime: {timeData.lastPlayedProgressTime}");
                Debug.Log($"time :: savedTime: {savedTime}");
                DateTime currentTime = DateTime.UtcNow;

                TimeSpan timeDiff = currentTime - savedTime;
                totalTimeOffInSeconds = timeDiff.TotalSeconds;
                Debug.Log($"time :: totalTimeOffInSeconds: {totalTimeOffInSeconds}");
                Debug.Log($"time :: timeData.lastElapsedSeconds: {timeData.lastElapsedSeconds}");
            }

            if (!string.IsNullOrWhiteSpace(timeData.lastElapsedSeconds))
            {
                prevTimeInSecondsRem = Double.Parse(timeData.lastElapsedSeconds);
            }

            Debug.Log($"time :: prevTimeInSecondsRem: {prevTimeInSecondsRem}");

            if (!IsFull)
                UpdateBasedOnSavedTime();
        }
        else
        {
            userHealthData.attainedLifes = availableLifes;
        }
    }

    private void UpdateBasedOnSavedTime()
    {
        var trackedSeconds = prevTimeInSecondsRem == 0 ? gameHealthData.timeInSecondsForOneLife : prevTimeInSecondsRem;
        if (totalTimeOffInSeconds > trackedSeconds)
        {
            totalTimeOffInSeconds -= trackedSeconds;
            prevTimeInSecondsRem = 0;
            AddHealth(1);
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
        if (!enableTimer || !startHealthTimer) 
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
                AddHealth(1); 
            }
        }
    }

    public string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSecondsRem);
        return time.ToString(@"mm\:ss");
    }

    public void RefillHealth()
    {
        availableLifes += gameHealthData.totalLifes - availableLifes;
    }

    public void AddHealth(int life)
    {
        if (life == 0)
        {
            Debug.Log($"Adding availableLifes: {0}");
        }

        #if UNITY_EDITOR
        if (availableLifes == gameHealthData.totalLifes) return;
        #endif

        Debug.Log($"Before adding lives: availableLifes: {availableLifes}");
        availableLifes += life;
        Debug.Log($"After adding lives: availableLifes: {availableLifes}");
        UpdateAvailableLives();
    }

    public void RemoveHealth(int life)
    {
        if (life == 0)
        {
            Debug.Log($"Removing availableLifes: {0}");
        }

        #if UNITY_EDITOR
        if (availableLifes == 0) return;
        #endif

        Debug.Log($"Before removing lives: availableLifes: {availableLifes}");
        availableLifes -= life;
        Debug.Log($"After removing lives: availableLifes: {availableLifes}");
        UpdateAvailableLives();
    }

    private void UpdateAvailableLives()
    {
        SetUserHealthData();
        globalHUDScreen.UpdateLivesButtonContainer(IsFull);

        Debug.Log($"Available lives: {availableLifes}, getMoreLivesPopup: {getMoreLivesPopup}");

        if (IsFull)
        {
            Debug.Log($"IsFull: ");
            startHealthTimer = false;
            getMoreLivesPopup?.SetHealthContent(true);
            if (getMoreLivesPopup != null && getMoreLivesPopup.gameObject.activeInHierarchy)
            {
                getMoreLivesPopup.ResetContentScale();
            }       
            
            totalSecondsRem = gameHealthData.timeInSecondsForOneLife;
            SetLastProgressTime();

            return;
        }

        Debug.Log($"!IsFull: ");

        if (getMoreLivesPopup && !getMoreLivesPopup.IsLifeToFillContentActive)
            getMoreLivesPopup.SetHealthContent(false);

        if (getMoreLivesPopup != null && getMoreLivesPopup.gameObject.activeInHierarchy)
            getMoreLivesPopup.UpdateAvailableLifes(availableLifes);

        Debug.Log($"life totalTimeOffInSeconds: {totalTimeOffInSeconds}");
        if (totalTimeOffInSeconds > 0)
            UpdateBasedOnSavedTime();
        else 
        {
            Debug.Log($"life hasTimerFilled: {hasTimerFilled}");
            Debug.Log($"life totalSecondsRem: {totalSecondsRem}");
            totalSecondsRem = hasTimerFilled ? gameHealthData.timeInSecondsForOneLife : totalSecondsRem;
            hasTimerFilled = false;
            startHealthTimer = true;
        }
    }

    public void SetLastProgressTime(string utcNow = "")
    {
        if (gameHealthData != null && IsFull) 
        {
            Debug.Log($"utcNow: {utcNow}, resetting elapsed seconds");
            startHealthTimer = false;
            userDataBehaviour?.SetLastProgressTime(utcNow, $"{0}");
            return;
        }
        
        userDataBehaviour?.SetLastProgressTime(utcNow, $"{totalSecondsRem}");
    }

    private void SetUserHealthData()
    {
        if (userDataBehaviour)
        {
            userHealthData.attainedLifes = availableLifes;
            userDataBehaviour.SetUserHealthData(userHealthData);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log($"ExitCallback OnApplicationFocus");
        SetLastProgressTime();
    }

    private void OnApplicationQuit()
    {
        Debug.Log($"ExitCallback OnApplicationQuit");
        SetLastProgressTime(!IsFull ? $"{DateTime.UtcNow}" : "");
    }
}
