using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPManager : MonoBehaviour
{
    public static PPManager PPInstance;
    private Volume v;

    // Post processing effects
    [HideInInspector]
    public Bloom b;

    [HideInInspector]
    public Vignette vg;

    [HideInInspector]
    public WhiteBalance wb;

    [HideInInspector]
    public MotionBlur mb;

    [HideInInspector]
    public ChromaticAberration ca;

    [HideInInspector]
    public LensDistortion ld;

    [HideInInspector]
    public DepthOfField dof;

    #region Initials
    [HideInInspector] public float startVg;
    [HideInInspector] public float startCa;
    [HideInInspector] public float startLd;
    [HideInInspector] public float startDof;
    #endregion

    #region Coroutines
    public Coroutine vignetteCor;
    public Coroutine ldCor;
    public Coroutine caCor;
    public Coroutine dofCor;
    #endregion

    void Awake()
    {
        PPInstance = this;

        v = GetComponent<Volume>();
        
        v.profile.TryGet(out b);
        v.profile.TryGet(out vg);
        v.profile.TryGet(out wb);
        v.profile.TryGet(out mb);
        v.profile.TryGet(out ca);
        v.profile.TryGet(out ld);
        v.profile.TryGet(out dof);
    }

    void Start()
    {
        if (vg != null)
        {
            startVg = vg.intensity.value;
        }
        
        if (ca != null)
        {
            startCa = ca.intensity.value;
        }

        if (ld != null)
        {
            startLd = ld.intensity.value;
        }

        if (dof != null)
        {
            startDof = dof.focalLength.value;
        }
    }

    public void StopPPCoroutines()
    {
        vg.intensity.value = startVg;

        if (vignetteCor != null) StopCoroutine(vignetteCor);
        if (caCor != null) StopCoroutine(caCor);
        if (ldCor != null) StopCoroutine(ldCor);
        if (dofCor != null) StopCoroutine(dofCor);
    }

    public IEnumerator PingPongVignette(float end, float duration)
    {
        float timeElapsed = 0;
        float normalizedTime = 0;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;

            vg.intensity.Interp(startVg, end, Mathf.PingPong(normalizedTime, 0.5f));

            yield return null;
        }
        while(timeElapsed < duration);

        vg.intensity.value = startVg;
    }

    public IEnumerator LerpVignette(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;

        float initialVg = vg.intensity.value;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            normalizedTime = Easing.InCubic(normalizedTime);

            vg.intensity.Interp(initialVg, end, normalizedTime);

            yield return null;
        }
        while(timeElapsed < duration);
    }

    public IEnumerator PingPongChromattic(float end, float duration)
    {
        float timeElapsed = 0;
        float normalizedTime = 0;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;

            ca.intensity.Interp(startCa, end, Mathf.PingPong(normalizedTime, 0.5f));

            yield return null;
        }
        while(timeElapsed < duration);
    }

    public IEnumerator LerpChromatticAbberation(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;

        float initialCa = ca.intensity.value;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            normalizedTime = Easing.InCubic(normalizedTime);

            ca.intensity.Interp(initialCa, end, normalizedTime);

            yield return null;
        }
        while(timeElapsed < duration);
    }

    public IEnumerator LerpDOF(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;

        float initialDof = dof.focalLength.value;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            normalizedTime = Easing.InCubic(normalizedTime);

            dof.focalLength.Interp(initialDof, end, normalizedTime);

            yield return null;
        }
        while(timeElapsed < duration);
    }

    public IEnumerator LerpLensDistortion(float end, float duration)
    {
        float timeElapsed = 0f;
        float normalizedTime = 0f;

        float initialLd = ld.intensity.value;

        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            normalizedTime = Easing.InCubic(normalizedTime);

            ld.intensity.Interp(initialLd, end, normalizedTime);

            yield return null;
        }
        while(timeElapsed < duration);
    }
}
