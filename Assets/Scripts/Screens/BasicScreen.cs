using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BasicScreen : MonoBehaviour
{
    protected CanvasGroup _group;

    [SerializeField] protected float _transitionDuration;

    public virtual void Awake()
    {
        _group = GetComponent<CanvasGroup>();
    }

    public virtual void Restart()
    {
        SceneHandler.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public virtual void GoToMenu()
    {
        SceneHandler.Instance.LoadLevel(0);
    }

    public void PlaySound(AudioSource source)
    {
        source.Play();
    }

    public void PlaySoundPositive(AudioSource source)
    {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();
    }

    public void PlaySoundNegative(AudioSource source)
    {
        source.pitch = 0.9f;
        source.Play();
    }
}
