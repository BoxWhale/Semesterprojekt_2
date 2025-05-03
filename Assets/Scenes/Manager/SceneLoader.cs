using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static readonly string sceneName_LoadingScreen = "LoadingScreen";
    public static readonly string sceneName_ManagementScreen = "Management";

    /// <summary>
    /// Loads a level and unloads all other levels except the Management scene.
    /// ShowLoadingScreen() MUST be called after this method to show the loading screen and to run correctly.
    /// </summary>
    /// <param name="levelName"> The name of the level in string "case sensitive"</param>
    public static void LoadLevel(string levelName)
    {
        LoadingScreen.Tasks.Push(CO_LoadLevel(levelName));
        UnloadAllScenesExcept("Management");
    }

    public static void UnloadAllScenesExcept(params string[] scenes)
    {
        LoadingScreen.Tasks.Push(CO_UnloadAllScenesExcept(scenes));
    }

    private static IEnumerator CO_LoadLevel(string levelName)
    {
        yield return "Loading " + levelName;
        AsyncOperation op = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        while (!op.isDone) yield return op.progress;
    }

    private static IEnumerator CO_UnloadAllScenesExcept(params string[] scenes)
    {
        yield return "Unloading scenes";
        var scenesToUnload = new List<AsyncOperation>();
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene_i = SceneManager.GetSceneAt(i);
            if (scenes.Any(sceneName => scene_i.name == sceneName) || scene_i.name == sceneName_LoadingScreen)
                continue;
            scenesToUnload.Add(SceneManager.UnloadSceneAsync(scene_i));
        }

        while (scenesToUnload.Any(op => !op.isDone)) 
            yield return -1;
    }

    public static bool IsLoading()
    {
        return SceneManager.GetSceneByName(sceneName_LoadingScreen).IsValid();
    }

    /// <summary>
    /// Opens the loading screen if it is not already open and there are tasks to run.
    /// MUST be called after LoadLevel() to show the loading screen and to run correctly.
    /// </summary>
    public static void ShowLoadingScreen()
    {
        if (!IsLoading() && LoadingScreen.Tasks.Count > 0)
            SceneManager.LoadSceneAsync(sceneName_LoadingScreen, LoadSceneMode.Additive);
    }

    public static void RemoveLoadingScreen()
    {
        SceneManager.UnloadSceneAsync(sceneName_LoadingScreen);
    }
    public static string GetCurrentLevelSceneName()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneName_LoadingScreen && scene.name != sceneName_ManagementScreen && scene.isLoaded)
            {
                return scene.name;
            }
        }
        return null; // or throw, or return a default
    }
}