using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    [SerializeField] private AudioMixer _mixer;

    #region coroutines
    private Coroutine _pitchCor;
    private Coroutine _lowpassCor;
    private Coroutine _volumeCor;
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);

            return;
        }

        Destroy(this.gameObject);
    }

    public void ResetAudio()
    {
        _mixer.SetFloat("MEPitch", 1f);
        _mixer.SetFloat("MELowpass", 22000f);
    }

    public void ResetAudioLerp(float duration)
    {
        LerpPitch(1f, duration);
        LerpLowpass(22000f, duration);
    }

    public void StopAllAudioCoroutines()
    {
        if (_lowpassCor != null)
            StopCoroutine(_lowpassCor);
        
        if (_pitchCor != null)
            StopCoroutine(_pitchCor);

        if (_volumeCor != null)
            StopCoroutine(_volumeCor);
    }

    public void LerpPitch(float end, float duration)
    {
        if (_pitchCor != null)
            StopCoroutine(_pitchCor);

        _pitchCor = StartCoroutine(PitchCor(end, duration));
    }

    private IEnumerator PitchCor(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;
        _mixer.GetFloat("MEPitch", out float startPitch);

        do
        {
            timeElapsed += Time.unscaledDeltaTime;
            normalizedTime = timeElapsed / duration;

            _mixer.SetFloat("MEPitch", Mathf.Lerp(startPitch, end, normalizedTime));

            yield return null;
        }
        while(timeElapsed < duration);
    }

    public void LerpLowpass(float end, float duration)
    {
        if (_lowpassCor != null)
            StopCoroutine(_lowpassCor);

        _lowpassCor = StartCoroutine(LowpassCor(end, duration));
    }

    private IEnumerator LowpassCor(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;
        _mixer.GetFloat("MELowpass", out float startPitch);

        do
        {
            timeElapsed += Time.unscaledDeltaTime;
            normalizedTime = timeElapsed / duration;

            _mixer.SetFloat("MELowpass", Mathf.Lerp(startPitch, end, normalizedTime));

            yield return null;
        }
        while(timeElapsed < duration);
    }

    public void ChangeLowpass(float end)
    {
        _mixer.SetFloat("MELowpass", end);
    }

    public void LerpVolume(float end, float duration)
    {
        if (_volumeCor != null)
            StopCoroutine(_volumeCor);

        _volumeCor = StartCoroutine(VolumeCor(end, duration));
    }

    private IEnumerator VolumeCor(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;
        _mixer.GetFloat("MEVolume", out float startVolume);

        do
        {
            timeElapsed += Time.unscaledDeltaTime;
            normalizedTime = timeElapsed / duration;

            _mixer.SetFloat("MEVolume", Mathf.Lerp(startVolume, end, normalizedTime));

            yield return null;
        }
        while(timeElapsed < duration);
    }
}
