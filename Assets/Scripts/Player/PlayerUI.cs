using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;
    
    
    [Header("Player UI variables")]
    [SerializeField] private TextMeshProUGUI _pelletText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _totalNoOfPellets;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ScoreManager.OnPelletCountUpdated += UpdatePelletText;

        _totalNoOfPellets = PelletManager.Instance.pellets.Count;
        UpdatePelletText();
    }

    private void Destroy()
    {
        ScoreManager.OnPelletCountUpdated -= UpdatePelletText;
    }

    private void Update()
    {
        _timerText.text = FormatTime(ScoreManager.Instance.timeSpent);
    }

    private void UpdatePelletText()
    {
        // Format the text as "numberOfPelletsCollected / Total number of pellets"
        _pelletText.text = string.Format("{0}/{1}", ScoreManager.Instance.pelletCount, _totalNoOfPellets);
    }

    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 1000f);

        string millesecondsFormatted = milliseconds.ToString().PadLeft(3, '0');
        string millesecondsFormattedTrimmed = millesecondsFormatted.Substring(0, 2);

        return string.Format("{0:D2}:{1:D2}:{2}", minutes, seconds, millesecondsFormattedTrimmed);
    }
}
