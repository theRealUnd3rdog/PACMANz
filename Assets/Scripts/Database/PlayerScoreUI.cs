using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameUI;
    public TextMeshProUGUI playerScoreUI;

    public void NewScoreElement(string username, string score)
    {
        playerNameUI.text = username;
        playerScoreUI.text = score;
    }
}
