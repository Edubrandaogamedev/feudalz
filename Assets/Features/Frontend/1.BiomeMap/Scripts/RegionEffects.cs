using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class RegionEffects : MonoBehaviour
{
    [Header("Feedbacks")]
    [Tooltip("This will work as a sequencer. So put in order")]
    [SerializeField] private MMFeedbacks feedbackFallingCenter;
    [SerializeField] private MMFeedbacks feedbackGoldzEffect;
    private void OnEnable()
    {
        feedbackFallingCenter.Events.OnComplete.AddListener(ChangeToOrtographicView);
    }
    private void OnDisable() {
        feedbackFallingCenter.Events.OnComplete.RemoveListener(ChangeToOrtographicView);
    }
    public void PlayFallingFeedback()
    {
        feedbackFallingCenter.PlayFeedbacks();
    }
    private void ChangeToOrtographicView()
    {
        Camera.main.orthographic = true;
        feedbackGoldzEffect.PlayFeedbacks();
    }
}
