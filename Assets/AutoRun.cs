using System;
using UnityEngine;

public class AutoRun : MonoBehaviour
{
    private void Start()
    {
        SceneLoader.LoadLevel("MainMenu");
        SceneLoader.ShowLoadingScreen();
    }
}
