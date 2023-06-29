using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class RestartScreen : BasicScreen
{
    public static RestartScreen Instance;
    private Coroutine _restartCoroutine;

    [Header("Audio")]
    [SerializeField] private AudioSource _deadScreenSource;

    public override void Awake()
    {
        Instance = this;

        base.Awake();
    }

    public override void Restart()
    {
        if (_restartCoroutine != null)
            StopCoroutine(_restartCoroutine);

        base.Restart();
    }

    public override void GoToMenu()
    {
        if (_restartCoroutine != null)
            StopCoroutine(_restartCoroutine);

        base.GoToMenu();
    }

    public void ShowRestart()
    {
        _group.blocksRaycasts = true;
        _group.interactable = true;

        // post processing
        PPManager.PPInstance.StopPPCoroutines();
        StartCoroutine(PPManager.PPInstance.LerpVignette(0.42f, _transitionDuration));
        StartCoroutine(PPManager.PPInstance.LerpLensDistortion(-0.4f, _transitionDuration));
        StartCoroutine(PPManager.PPInstance.LerpChromatticAbberation(0.35f, _transitionDuration));

        // audio
        if (_deadScreenSource != null)
            _deadScreenSource.Play();

        AudioManager._instance.LerpLowpass(5000f, _transitionDuration);
        AudioManager._instance.LerpPitch(0.5f, _transitionDuration);

        _restartCoroutine = StartCoroutine(RestartScreenTransition(_transitionDuration));
    }

    private IEnumerator RestartScreenTransition(float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;
        float timeScaleDuration = 0f;

        // time scales
        float startTimeScale = Time.timeScale;
        float startFixedTime = Time.fixedDeltaTime;

        do
        {
            timeElapsed += Time.unscaledDeltaTime;
            normalizedTime = timeElapsed / duration;
            timeScaleDuration = timeElapsed / (duration/4);

            Time.timeScale = Mathf.Lerp(1f, 0.3f, timeScaleDuration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _group.alpha = Mathf.Lerp(0, 1f, normalizedTime);

            yield return null;
        }
        while (timeElapsed < duration);
    }
}
