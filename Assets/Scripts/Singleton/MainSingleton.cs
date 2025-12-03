using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton : MonoBehaviour {}

public class MainSingleton : MonoBehaviour
{
    public static MainSingleton Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            DestroyImmediate(gameObject);
        
        DontDestroyOnLoad(Instance);
    }

    public void LoadMenuLoadingScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGameplayScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
