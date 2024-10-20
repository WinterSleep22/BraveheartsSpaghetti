using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class SoundManagerMenu : MonoBehaviour {

    static SoundManagerMenu _instance;
    public static SoundManagerMenu instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManagerMenu>();
            }
            return _instance;
        }
    }
    public AudioSource onFirstBackgroundHides;
    public AudioSource goSound;
    public AudioSource backSound;
	public AudioSource incorrectSound;

    void Start()
    {
        //only one instance
        if (FindObjectsOfType<SoundManagerMenu>().Length == 2)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        AssignAllButtonOnClickSound();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        AssignAllButtonOnClickSound();
    }


    void AssignAllButtonOnClickSound()
    {
        //fast way to add sound to the game
        //just  a test
        MenuManager.instance.transform.GetComponentsInChildrenBFS<Button>(true).ToList().ForEach(
            b => {
                if(
                    b.gameObject.name.Contains("Back") || 
                    b.gameObject.name.Contains("back") || 
                    b.gameObject.name.Contains("cancel") || 
                    b.gameObject.name.Contains("Cancel")
                   )
                {
                    
                    b.onClick.AddListener(BackSound);
                }
                else
                {
                    b.onClick.AddListener(GoSound);
                }
            }
            );
    }

    public void GoSound()
    {
        goSound.Play();
    }
    public void BackSound()
    {
        backSound.Play();
    }
}
