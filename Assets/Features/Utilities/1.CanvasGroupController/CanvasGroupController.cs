using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupController : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Coroutine disableAfterTime;
    Coroutine enableAfterTime;

    event Action OnEnable;
    event Action OnDisable;

    public bool Enabled => canvasGroup?.interactable ?? GetReference().interactable;


    [System.Serializable]
    public class Events
    {
        public UnityEvent onEnable; 
        public UnityEvent onDisable;
        public UnityEvent onStartAppearing;
        public UnityEvent onStartDisappearing;
    }
    [SerializeField] Events events = null;

    public void StartListeningToOnEnableEvent(Action _action)
    {
        OnEnable += _action;
    }
    public void StopListeningToOnEnableEvent(Action _action)
    {
        OnEnable -= _action;
    }
    public void StartListeningToOnDisableEvent(Action _action)
    {
        OnDisable += _action;
    }
    public void StopListeningToOnDisableEvent(Action _action)
    {
        OnDisable -= _action;
    }

    public void EnableRaycast()
    {
        if (canvasGroup == null)
            GetReference();
        canvasGroup.blocksRaycasts = true;
    }
    public void DisableRaycast()
    {
        if (canvasGroup == null)
            GetReference();
        canvasGroup.blocksRaycasts = false;
    }
    public void EnableInteractable()
    {
        if (canvasGroup == null)
            GetReference();
        canvasGroup.interactable = true;
    }
    public void DisableInteractable()
    {
        if (canvasGroup == null)
            GetReference();
        canvasGroup.interactable = false;
    }
    public void EnableImage()
    {
        if (canvasGroup == null)
            GetReference();
        canvasGroup.alpha = 1;
    }
    public void DisableImage()
    {
        if (canvasGroup == null)
            GetReference();
        canvasGroup.alpha = 0;
    }
    public void Enable()
    {
        if (canvasGroup == null)
            GetReference();

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        events.onEnable.Invoke();
        OnEnable?.Invoke();
    }
    //public void EnableAfterTime(float time)
    //{
    //    if (enableAfterTime != null)
    //        StopCoroutine(enableAfterTime);
    //    enableAfterTime = IEnumeratorHelper.DoAfterTime(this, Enable, time);
    //}
    //public void EnableAfterUnscaledTime(float time)
    //{
    //    if (enableAfterTime != null)
    //        StopCoroutine(enableAfterTime);
    //    enableAfterTime = IEnumeratorHelper.DoAfterUnscaledTime(this, Enable, time);
    //}

    public void StartSoftEnable()
    {
        if (canvasGroup == null)
            GetReference();

        StartCoroutine(SoftEnable());
    }
    IEnumerator SoftEnable()
    {
        float time = 0;
        while (time < 0.2f)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Min(time, 0.2f) * 5;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        events.onEnable.Invoke();
    }
    //public void StartSoftEnableAfterTime(float time)
    //{
    //    if (enableAfterTime != null)
    //        StopCoroutine(enableAfterTime);
    //    enableAfterTime = IEnumeratorHelper.DoAfterTime(this, StartSoftEnable, time);
    //}
    //public void StartSoftEnableAfterUnscaledTime(float time)
    //{
    //    if (enableAfterTime != null)
    //        StopCoroutine(enableAfterTime);
    //    enableAfterTime = IEnumeratorHelper.DoAfterUnscaledTime(this, StartSoftEnable, time);
    //}

    public void Disable()
    {
        if (canvasGroup == null)
            GetReference();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        events.onDisable.Invoke();
        OnDisable?.Invoke();
    }
    //public void DisableAfterTime(float time)
    //{
    //    if (disableAfterTime != null)
    //        StopCoroutine(disableAfterTime);
    //    disableAfterTime = IEnumeratorHelper.DoAfterTime(this, Disable, time);
    //}
    //public void DisableAfterUnscaledTime(float time)
    //{
    //    if (disableAfterTime != null)
    //        StopCoroutine(disableAfterTime);
    //    disableAfterTime = IEnumeratorHelper.DoAfterUnscaledTime(this, Disable, time);
    //}

    public void StartSoftDisable(float _duration)
    {
        if (canvasGroup == null)
            GetReference();

        if (canvasGroup.interactable == false)
            return;

        StartCoroutine(SoftDisable(_duration));
    }
    public void StartSoftDisable()
    {
        if (canvasGroup == null)
            GetReference();

        StartCoroutine(SoftDisable(0.2f));
    }
    IEnumerator SoftDisable(float _duration)
    {
        canvasGroup.blocksRaycasts = false;
        float time = _duration;
        while (time > 0)
        {
            yield return null;
            time -= Time.unscaledDeltaTime;
            if(canvasGroup.alpha != 0)
            {
                canvasGroup.alpha = Mathf.Max(time / _duration, 0);
            }          
        }
        canvasGroup.interactable = false;
        events.onDisable.Invoke();
    }
    //public void StartSoftDisableAfterTime(float time)
    //{
    //    if (disableAfterTime != null)
    //        StopCoroutine(disableAfterTime);
    //    disableAfterTime = IEnumeratorHelper.DoAfterTime(this, StartSoftDisable, time);
    //}
    //public void StartSoftDisableAfterUnscaledTime(float time)
    //{
    //    if (disableAfterTime != null)
    //        StopCoroutine(disableAfterTime);
    //    disableAfterTime = IEnumeratorHelper.DoAfterUnscaledTime(this, StartSoftDisable, time);
    //}

    public void StartAppearing(float duration)
    {
        StartCoroutine(Appear(duration));
    }
    IEnumerator Appear(float duration)
    {
        if (canvasGroup == null)
            GetReference();
        events.onStartAppearing.Invoke();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        float time = 0;
        while (time < duration)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Min(time / duration, duration);
        }
        Enable();
    }
    public void StartDisappearing(float duration)
    {
        StartCoroutine(Disappear(duration));
    }
    IEnumerator Disappear(float duration)
    {
        if (canvasGroup == null)
            GetReference();
        events.onStartAppearing.Invoke();
        float time = 0;
        while (time < duration)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Max(1.0f - time / duration, 0);
        }
        Disable();
    }

    public void Toggle()
    {
        if (canvasGroup == null)
            GetReference();

        if (!canvasGroup.interactable)
            Enable();
        else
            Disable();
    }

    public bool isVisible()
    {
        if (canvasGroup?.alpha >= 1)
        {
            return true;
        } else
        {
            return false;
        }
    }
    //public void ToogleAfterTime(float time)
    //{
    //    IEnumeratorHelper.DoAfterTime(this, Toogle, time);
    //}

    private CanvasGroup GetReference()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        return canvasGroup;
    }
}
