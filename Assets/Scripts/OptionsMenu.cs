using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void Back()
    {
        gameObject.SetActive(false);
        GameObject.Find("MainMenu").SetActive(true);
    }

    public void arrowkey()
    {

    }

    public void WASD()
    {

    }
}
