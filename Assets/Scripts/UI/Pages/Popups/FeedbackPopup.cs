using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI feedbackText;

    public void SetFeedbackText(string feedbackTxt)
    {
        feedbackText.text = feedbackTxt;
    }

    public void OnAnimationComplete()
    {
        popupManager.HidePopup(popupType);
    }
}
