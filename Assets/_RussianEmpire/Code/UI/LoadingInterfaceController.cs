using System;
using System.Collections;
using System.Collections.Generic;
using Railway.Events;
using Railway.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingInterfaceController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private GameObject camera;

    [SerializeField] private BoolEventChannelSO loadingScreenToggleEvent;
    [SerializeField] private FloatEventChannelSO loadingProgressEvent;

    private Coroutine _smoothFillRoutine;

    private void OnEnable()
    {
        loadingScreenToggleEvent.OnEventRaised += ToggleLoadingScreen;
    }

    private void OnDisable()
    {
        loadingScreenToggleEvent.OnEventRaised -= ToggleLoadingScreen;
    }
    
    private void ToggleLoadingScreen(bool isActive)
    {
        camera.SetActive(isActive);
        loadingScreen.SetActive(isActive);

        StartCoroutine(SmoothFill(2));
    }
    
    private IEnumerator SmoothFill(float target)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startValue = loadingBar.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingBar.value = Mathf.Lerp(startValue, target, elapsed / duration);
            yield return null;
        }

        loadingBar.value = target;
    }
}

