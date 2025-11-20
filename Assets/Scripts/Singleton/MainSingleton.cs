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

    public void LoadGameplayScene()
    {
        SceneManager.LoadScene(1);
    }
}
