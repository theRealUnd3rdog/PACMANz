using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    //hide main menu gameobject and show the level select gameobject
    public void LevelSelect()
    {
        gameObject.SetActive(false);
        GameObject.Find("/LevelSelection").SetActive(true);
    }

    //hide main menu gameobject and show the options gameobject
    public void Options()
    {
        gameObject.SetActive(false);
        GameObject.Find("Options").SetActive(true);
    }

    //hide main menu and show leaderboard gameobject
    public void Leaderboard()
    {
        gameObject.SetActive(false);
        GameObject.Find("Leaderboard").SetActive(true);
    }
}
