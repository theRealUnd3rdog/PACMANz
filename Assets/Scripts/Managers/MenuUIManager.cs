using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject menuUI;
    public GameObject leaderboardUI;
    public GameObject creditsUI;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _versionText;
    [SerializeField] private TextMeshProUGUI _welcomeText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(FirebaseManager.DatabaseManager.InitializeAndLogin());

        _versionText.text = $"V {Application.version}";
    }

    public void ClearScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        menuUI.SetActive(false);
        leaderboardUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    public void LoginScreen()
    {
        ClearScreen();
        loginUI.SetActive(true);
    }

    public void RegisterScreen()
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void UserDataScreen()
    {
        ClearScreen();
        menuUI.SetActive(true);

        _welcomeText.text = $"WELCOME {FirebaseManager.DatabaseManager.User.DisplayName}";
    }

    public void ShowLeaderboard()
    {
        ClearScreen();
        leaderboardUI.SetActive(true);
    }

    public void ShowCredits()
    {
        ClearScreen();
        creditsUI.SetActive(true);
    }

    public void Play()
    {
        SceneHandler.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit() => Application.Quit();

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
