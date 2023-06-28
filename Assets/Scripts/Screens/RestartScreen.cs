using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class RestartScreen : MonoBehaviour
{
    public static RestartScreen Instance;

    // UI
    private CanvasGroup _restartGroup;
    private Coroutine _restartCoroutine;

    [Header("UI")]
    [SerializeField] private float _transitionDuration;

    [Header("Audio")]
    [SerializeField] private AudioSource _deadScreenSource;

    private void Awake()
    {
        Instance = this;

        _restartGroup = GetComponent<CanvasGroup>();
    }

    public void Restart()
    {
        if (_restartCoroutine != null)
            StopCoroutine(_restartCoroutine);

        SceneHandler.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {

    }

    public void ShowRestart()
    {
        _restartGroup.blocksRaycasts = true;
        _restartGroup.interactable = true;

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
            _restartGroup.alpha = Mathf.Lerp(0, 1f, normalizedTime);

            yield return null;
        }
        while (timeElapsed < duration);
    }
}
