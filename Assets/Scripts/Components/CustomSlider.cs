using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Start()
    {
        slider.value = 0;
    }

    void Update()
    {
        slider.value += Time.deltaTime / 2;
        if (slider.value >= 1)
        {
            MainSingleton.Instance.LoadMenuScene();
        }
    }
}
