using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlipOnClick : MonoBehaviour {

    CardGraphics cg;
    public delegate void onCardFlipped();
    public static event onCardFlipped cardFlipped;
    public static int total;

    void Awake()
    {
        cg = GetComponent<CardGraphics>();
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    bool toreturn = false;
    public void OnPointerDownDelegate(PointerEventData data)
    {
        if (cg == null)
            cg = GetComponent<CardGraphics>();
        if (toreturn)
            return;
        if (cg.isFlipped)
        {
            cg.FlipCard();
            StartCoroutine(InvokeEvent());
        }
        toreturn = true;
    }

    IEnumerator InvokeEvent()
    {
        while (true)
        {
            if (cg.isShowingFront)
            {
                total++;
                if (total == 5)
                {
                    if (cardFlipped != null)
                        cardFlipped.Invoke();
                    total = 0;
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
