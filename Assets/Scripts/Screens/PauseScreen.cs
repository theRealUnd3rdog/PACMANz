using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : BasicScreen
{
    public static bool GameIsPaused = false;
    private Coroutine _pauseCor;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {                    
                Resume();
            }
            else
                Pause();
        }
    }

    public void Resume()
    {
        _group.blocksRaycasts = false;
        _group.interactable = false;

        // time scales and audio effects
        PauseWithTransition(1f, _transitionDuration / 2);
        AudioManager._instance.LerpPitch(1f, 0.5f);
        GameIsPaused = false;
    }

    public void Pause()
    {
        _group.blocksRaycasts = true;
        _group.interactable = true;

        // time scales and audio effects
        PauseWithTransition(0f, _transitionDuration);
        AudioManager._instance.LerpPitch(0f, 0.8f);
        GameIsPaused = true;
    }

    private void PauseWithTransition(float end, float duration)
    {
        if (_pauseCor != null)
            StopCoroutine(_pauseCor);

        _pauseCor = StartCoroutine(LerpTimescale(end, duration));
    }

    private IEnumerator LerpTimescale(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;

        // time scales
        float startTimeScale = Time.timeScale;
        
        float startAlpha = _group.alpha;

        do
        {
            timeElapsed += Time.unscaledDeltaTime;
            normalizedTime = timeElapsed / duration;

            Time.timeScale = Mathf.Lerp(startTimeScale, end, normalizedTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _group.alpha = Mathf.Lerp(startAlpha, end == 0? 1: 0, normalizedTime);

            yield return null;
        }
        while (timeElapsed < duration);
    }
}
