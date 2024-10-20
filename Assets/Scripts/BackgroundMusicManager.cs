using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TappxSDK;

public class BackgroundMusicManager : MonoBehaviour {
    public const string menuSceneName = "MainMenu";
    public const string gameSceneName = "Game";
    public AudioSource menuMusic;
    public AudioSource gameMusic;
    static BackgroundMusicManager _instance;

    public static BackgroundMusicManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<BackgroundMusicManager>();
            return _instance;
        }
    }

    void Start()
    {
        //only one instance
        if (FindObjectsOfType<BackgroundMusicManager>().Length == 2)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    void OnSceneChange(Scene arg1, Scene arg2)
    {
        menuMusic.Stop();
        gameMusic.Stop();
        // Debug.Log("arg1: " + arg1.name + "  arg2: " + arg2.name);
        switch (arg2.name)
        {
            case menuSceneName:
                menuMusic.Play();
                break;
            // case gameSceneName:
            //     break;
            default:
                Debug.LogWarning("Couldn't play background music!; music not set ", this);
                break;
        }

    }
}
