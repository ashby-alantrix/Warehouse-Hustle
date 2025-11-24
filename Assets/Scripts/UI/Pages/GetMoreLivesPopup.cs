using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetMoreLivesPopup : UIBase
{
    [SerializeField] private SpriteRenderer[] lifeSprites;

    [SerializeField] private Button purchaseBtn;
    [SerializeField] private int currencyValue = 50;
    [SerializeField] private TextMeshProUGUI timerText;

    private int currentEnabledLifeIcons;
    private HealthSystem healthSystem;

    private void OnEnable() 
    {
        healthSystem = InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>();
        currentEnabledLifeIcons = healthSystem.AvailableLifes;

        for (int indexI = 0; indexI < lifeSprites.Length; indexI++)
            lifeSprites[indexI].enabled = indexI <= currentEnabledLifeIcons - 1;
    }

    public void UpdateAvailableLifes(int life)
    {
        currentEnabledLifeIcons += life;

        if (Mathf.Sign(life) > 0)
            lifeSprites[currentEnabledLifeIcons - 1].enabled = true;
        else 
            lifeSprites[currentEnabledLifeIcons].enabled = false;
    }

    void Update()
    {
        timerText.text = healthSystem.GetFormattedTime();
    }
}
