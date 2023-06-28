using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private Animator _animator;
    public static SceneHandler Instance;

    [SerializeField] private float _transitionTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _animator = GetComponent<Animator>();

            DontDestroyOnLoad(this.gameObject);

            return;
        }

        Destroy(this.gameObject);
    }

    public void LoadLevel(int sceneIndex)
    {
        if (_animator != null)
            _animator.SetBool("Transition", true);

        AudioManager._instance.LerpVolume(-80f, _transitionTime);
        StartCoroutine(LoadSceneAsychrnously(sceneIndex));
    }

    private IEnumerator LoadSceneAsychrnously(int sceneIndex)
    {
        yield return new WaitForSecondsRealtime(_transitionTime);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log("Progress: " + progress);

            yield return null;
        }

        TimeManager.ResetTime();

        // reset the audio
        if (AudioManager._instance != null)
        {
            AudioManager._instance.ResetAudioLerp(_transitionTime);
            AudioManager._instance.LerpVolume(0f, 0.5f);
        }

        if (_animator != null)
            _animator.SetBool("Transition", false);
    }
}
