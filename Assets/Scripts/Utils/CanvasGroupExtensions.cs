using UnityEngine;
using System.Collections;

public static class CanvasGroupExtensions  {


    /// <summary>
    /// creates a coroutine to fade to alpha property on fixed delta time velocity 
    /// </summary>
    /// <param name="me"></param>
    /// <param name="initialAlpha"></param>
    /// <param name="finalAlpha"></param>
    /// <param name="time"></param>
    /// <param name="mono"></param>
    public static void Fade(this CanvasGroup me,float initialAlpha,float finalAlpha,float time,MonoBehaviour mono)
    {
        mono.StartCoroutine( Fading(me,initialAlpha,finalAlpha,time) );
    }
    public static IEnumerator Fading(CanvasGroup me, float initialAlpha, float finalAlpha, float time)
    {
        float currentTime = 0;
        for (; ; )
        {
            me.alpha = Mathf.Lerp(initialAlpha,finalAlpha,currentTime/time);
            currentTime += Time.fixedDeltaTime;
            if(currentTime > time){
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
