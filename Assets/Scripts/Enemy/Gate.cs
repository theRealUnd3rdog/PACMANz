using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Gate : MonoBehaviour
{
    private Material _gateVisual;

    [Header("Audio")]
    [SerializeField] private AudioSource _gateSound;

    private void Awake()
    {
        _gateVisual = this.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        StartCoroutine(OpenGate());
    }

    private IEnumerator OpenGate()
    {
        yield return new WaitForSeconds(EnemyManager.Instance.gateDuration);

        CameraShaker.Instance.ShakeOnce(5f, 0.3f, 0.4f, 7f);

        if (_gateSound != null)
        {
            _gateSound.pitch = Random.Range(0.9f, 1.1f);
            _gateSound.Play();
        }

        float timeElapsed = 0f;
        float normalizedTime = 0f;
        float transitionDuration = 0.5f; // << Adjust this to change the duration for the gate to fade
        float startTransparancy = _gateVisual.GetFloat("_Transparancy");

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / transitionDuration;

            _gateVisual.SetFloat("_Transparancy", Mathf.Lerp(startTransparancy, 0f, normalizedTime));

            yield return null;
        }
        while (timeElapsed < transitionDuration);
    }
}
