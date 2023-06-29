using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : BasicScreen
{
    public static WinScreen Instance;
    private Coroutine _winCoroutine;

    public override void Awake()
    {
        Instance = this;

        ScoreManager.OnPelletCountCompleted += ShowWinScreen;

        base.Awake();
    }

    public void OnDestroy()
    {
        ScoreManager.OnPelletCountCompleted -= ShowWinScreen;
    }

    public override void Restart()
    {
        if (_winCoroutine != null)
            StopCoroutine(_winCoroutine);

        base.Restart();
    }

    public override void GoToMenu()
    {
        if (_winCoroutine != null)
            StopCoroutine(_winCoroutine);

        base.GoToMenu();
    }

    public void ShowWinScreen()
    {
        _group.blocksRaycasts = true;
        _group.interactable = true;

        // post processing
        PPManager.PPInstance.StopPPCoroutines();
        StartCoroutine(PPManager.PPInstance.LerpVignette(0.42f, _transitionDuration/2));
        StartCoroutine(PPManager.PPInstance.LerpLensDistortion(-0.4f, _transitionDuration/2));
        StartCoroutine(PPManager.PPInstance.LerpChromatticAbberation(0.35f, _transitionDuration/2));

        ScoreDBManager.Instance.SaveData(ScoreManager.Instance.timeSpent);
        ScoreDBManager.Instance.ScoreBoardButton();
        
        StartCoroutine(WinScreenTransition(_transitionDuration));
    }

    private IEnumerator WinScreenTransition(float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;

        do
        {
            timeElapsed += Time.unscaledDeltaTime;
            normalizedTime = timeElapsed / duration;

            _group.alpha = Mathf.Lerp(0, 1f, normalizedTime);

            yield return null;
        }
        while (timeElapsed < duration);
    }
}
