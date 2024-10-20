using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {

    public float time = 0.2f;
    public float distance = 10;

    Vector3 originalPosition;
    void Start()
    {
        originalPosition = transform.position;
    }
    
    void OnDisable()
    {
        SoundManager.instance.clock.Stop();
    }

    void OnEnable()
    {
        StartCoroutine(ZigZag());
        SoundManager.instance.clock.Play();
    }
    IEnumerator ZigZag()
    {
        transform.SetParent(GameMechanics.instance.myCanvas.transform);
        transform.SetAsLastSibling();
        for (; ; )
        {
           
            yield return new WaitForSeconds(time);
            iTween.MoveTo( gameObject, originalPosition + new Vector3(1,0,0)   * distance , time);
            yield return new WaitForSeconds(time);
            iTween.MoveTo( gameObject, originalPosition + new Vector3(-1,0,0) * distance , time);
        }
    }
}
