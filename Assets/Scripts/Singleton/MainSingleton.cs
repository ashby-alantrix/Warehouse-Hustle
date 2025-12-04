using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton : MonoBehaviour {}

public class MainSingleton : MonoBehaviour
{
    public static MainSingleton Instance;

    private ScreenManager screenManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            DestroyImmediate(gameObject);
        
        DontDestroyOnLoad(Instance);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        screenManager = screenManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>() : screenManager;

        switch (scene.name)
        {
            case WarehouseHustle_Constants.Menu_Loading_Scene:
                screenManager?.HideScreen(ScreenType.GlobalHUDScreen);
            break;
            case WarehouseHustle_Constants.Menu_Scene:
            case WarehouseHustle_Constants.Gameplay_Scene:
                screenManager?.ShowScreen(ScreenType.GlobalHUDScreen);
            break;
        }
    }

    public void LoadMenuLoadingScene()
    {
        SceneManager.LoadScene(WarehouseHustle_Constants.Menu_Loading_Scene);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(WarehouseHustle_Constants.Menu_Scene);
    }

    public void LoadGameplayScene()
    {
        SceneManager.LoadScene(WarehouseHustle_Constants.Gameplay_Scene);
    }
}
