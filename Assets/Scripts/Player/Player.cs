using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour , IKillable
{
    public static bool PlayerDead;
    private PlayerMovement _movement;

    [SerializeField] private GameObject _visual;

    [Header("Audio")]
    [SerializeField] private AudioSource _tapSource;
    [SerializeField] private AudioSource _deathSource;

    [Header("Player Ambience")]
    [SerializeField] private AudioSource _ambienceSource;
    [Range(0.1f, 0.5f)][SerializeField] private float _ambienceChangeDuration;
    private Coroutine _ambienceCoroutine;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _explosion;

    private void Awake()
    {
        PlayerDead = false;

        _movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _movement.OnCurrentDirectionChanged += PlayTapSound;
        _movement.OnMovementStateChanged += PlayAmbience;

        ScoreManager.OnPelletCountCompleted += StopMovement;
    }

    private void OnDestroy()
    {
        _movement.OnCurrentDirectionChanged -= PlayTapSound;
        _movement.OnMovementStateChanged -= PlayAmbience;

        ScoreManager.OnPelletCountCompleted -= StopMovement;
    }

    private void StopMovement()
    {
        _movement.StopMovement();
        _movement.enabled = false;
    }

    public void Kill()
    {
        if (PlayerDead)
            return;

        // Kill player
        StopMovement();

        if (_deathSource != null)
            _deathSource.Play();

        if (_ambienceSource != null)
            _ambienceSource.Stop();

        if (_explosion != null)
            _explosion.Play();

        _visual.SetActive(false);
        
        // Show restart screen
        EZCameraShake.CameraShaker.Instance.ShakeOnce(5f, 0.5f, 0.1f, 8f);
        RestartScreen.Instance.ShowRestart();

        // Stop scoremanager
        ScoreManager.StopScore = true;

        PlayerDead = true;
    }

    public void PlayTapSound()
    {
        if (_tapSource == null)
        {
            Debug.LogWarning("No tap sound!");
            return;
        }

        _tapSource.pitch = Random.Range(0.9f, 1.1f);
        _tapSource.PlayOneShot(_tapSource.clip);
    }

    public void PlayAmbience(MovementState state)
    {
        Debug.Log($"State: {state}");

        if (_ambienceCoroutine != null)
            StopCoroutine(_ambienceCoroutine);
        
        _ambienceCoroutine = StartCoroutine(AmbienceControl(state));
    }

    private IEnumerator AmbienceControl(MovementState state)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;
        float currentPitch = _ambienceSource.pitch;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / _ambienceChangeDuration;

            if (state == MovementState.Moving)
            {
                _ambienceSource.pitch = Mathf.Lerp(currentPitch, 1f, normalizedTime);
            }
            else if (state == MovementState.Stopped)
            {
                _ambienceSource.pitch = Mathf.Lerp(currentPitch, 0f, normalizedTime);
            }

            yield return null;
        }
        while (timeElapsed < _ambienceChangeDuration);
    }
}
