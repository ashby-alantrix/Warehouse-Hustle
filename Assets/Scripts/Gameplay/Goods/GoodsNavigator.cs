using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsNavigator : MonoBehaviour
{
    private bool hasTriggered = false;

    public void TriggerMovement(bool canTrigger = false)
    {
        hasTriggered = true;
        
    }

    private void Update()
    {
        if (hasTriggered)
        {
            
        }
    }
}
