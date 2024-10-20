using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class LogTextUI : MonoBehaviour {

    private static LogTextUI _instance;

    public static LogTextUI instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LogTextUI>();
            }
            return _instance;
        }
    }

    public Text myText;
   // public List<string> messages = new List<string>();

    public float maxSecondsShowMessage = 3;
    
    public static void Log(string message)
    {
        Debug.Log("### Log: " + message);
        // messages.Add(message);
        instance.myText.text = message;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.EraseMessageIn());
    }
 

    System.Collections.IEnumerator EraseMessageIn()
    {
        yield return new WaitForSeconds(maxSecondsShowMessage);
        myText.text = "";
    }

}
