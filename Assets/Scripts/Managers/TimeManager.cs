using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    private bool gamePaused = false;
    [SerializeField] public static float hitstopFrameDelay {get;} = 0.032f;
    private bool hitStopping = true;

    private void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else Destroy(this.gameObject);
    }

    public static void FreezeForFrames(int frameCount = 1)
    {
        Debug.Log("Void loaded");
        Instance.StartCoroutine(Instance.IEFreezeForFrames(frameCount));
    }

    public static void PauseGame()
    {
        Instance.StartCoroutine(Instance.IEPauseGame());
    }

    public static void ResumeGame()
    {
        ResetTime();
    }

    public static void ResetTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    private IEnumerator IEPauseGame()
    {
        do
            {yield return null;} 
        while 
            (hitStopping);
        Time.timeScale = 0;

        yield return null;
    }
    //Freeze for "frames" with a hitstop effect.
    //(default = 1 skipped frame at 30 FPS)
    private IEnumerator IEFreezeForFrames(int frameCount = 1)
    {
        Debug.Log("About to freeze");
        hitStopping = true;
        Time.timeScale = 0;
        for(int i = 0; i < frameCount; i++)
        {
            Debug.Log("Freezing");
        Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(hitstopFrameDelay);
        }
        Debug.Log("About to resume");
        Time.timeScale = 1;
    }
    
    private void Update()
    {
        #if UNITY_EDITOR
                if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Hitstop debug");
          TimeManager.FreezeForFrames(5);
        }
        #endif
    }
}
