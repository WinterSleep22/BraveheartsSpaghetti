using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCardsOpening : Menu
{

    [Space(7)]
    [SerializeField]
    float shakeDuration = 0.3f;
    [SerializeField]
    float shakeMagnitude = 50f;
    [Space(2)]
    [SerializeField]
    Sprite boxDefaultSprite;
    [SerializeField]
    Sprite boxNextSprite;
    [SerializeField]
    Button box;
    [SerializeField]
    Button confirmButton;
    [Space(2)]
    public CardGraphics displayedCardModel;
    public RectTransform[] cardsFinalPositions = new RectTransform[5];
    [Header("readonly variable")]
    [Space(10)]
    [SerializeField]
    Card[] pickedCards;
    public enum DropType { Normal, AleadyHave, Overbuy }
    public static DropType dropType()
    {
        DropType toReturn = DropType.Normal;
        Debug.Log(PersistentCardGame.GetTotalNumOfMoreCopiesToDrop());
        switch (PersistentCardGame.GetTotalNumOfMoreCopiesToDrop())
        {
            case 0:
                toReturn = DropType.AleadyHave;
                break;
            case 1:
                toReturn = DropType.Overbuy;
                break;
            case 2:
                toReturn = DropType.Overbuy;
                break;

            default:
                toReturn = DropType.Normal;
                break;
        }
        return toReturn;
    }

    List<GameObject> displayedCardsObjects;
    bool isShakingDone;

    public override void Show()
    {
        base.Show();
        ShowBanner();
        Initialize();
    }

    public override void Hide()
    {
        base.Hide();
        FlipOnClick.cardFlipped -= OnCardsFlipped;
        confirmButton.gameObject.SetActive(false);
        for (int i = 0; i < displayedCardsObjects.Count; i++)
            Destroy(displayedCardsObjects[i]);
        box.GetComponent<Button>().enabled = true;
        box.image.sprite = boxDefaultSprite;
    }

    void Initialize()
    {
        FlipOnClick.cardFlipped += OnCardsFlipped;
        box.enabled = true;
        isShakingDone = false;
        pickedCards = PickFiveCards();
        print(dropType());

        PersistentCardGame.AddOneToTotalCopiesAmount(pickedCards);
        PersistentCardGame.SetChestAmount(PersistentCardGame.GetChestAmount() - 1);
    }

    void OnCardsFlipped()
    {
        confirmButton.gameObject.SetActive(true);
        confirmButton.enabled = true;
    }

    public static Card[] Pool()
    {
        List<Card> _pool = new List<Card>(PersistentCardGame.GetDroppableCards());
        _pool.Remove(CardManager.GetCardPrefabByTitle("General"));
        _pool.Remove(CardManager.GetCardPrefabByTitle("Attack"));
        _pool.Remove(CardManager.GetCardPrefabByTitle("Defense"));
        return _pool.ToArray();
    }

    Card[] PickFiveCards()
    {
        List<Card> toReturn = new List<Card>();
        List<Card> allCards = new List<Card>(PersistentCardGame.GetAllCardsExceptGeAtDf());
        List<Card> cards = new List<Card>(Pool());
        allCards.Shuffle();
        cards.Shuffle();

        switch (dropType())
        {
            case DropType.Normal:
                for (int i = 0; i < 3; i++)
                    toReturn.Add(cards[i]);
                break;
            case DropType.Overbuy:
                for (int i = 0; i < cards.Count; i++)
                    toReturn.Add(cards[i]);
                int toDrop = 3 - toReturn.Count;
                for (int i = 0; i < toDrop; i++)
                    toReturn.Add(allCards[i]);
                break;
            case DropType.AleadyHave:
                for (int i = 0; i < 3; i++)
                    toReturn.Add(allCards[i]);
                break;
        }

        for (int i = 0; i < 2; i++)
        {
            //75% chance of AttackCard, 25% of DefenseCard
            float rand = Random.Range(0f, 1f);
            if (rand <= 0.75f)
                toReturn.Add(CardManager.GetCardPrefabByTitle("Attack"));
            else toReturn.Add(CardManager.GetCardPrefabByTitle("Defense"));
        }

        return toReturn.ToArray();

    }

    public void OpenBoxButtonClick()
    {
        box.enabled = false;
        StartCoroutine(OpenBox());
    }

    IEnumerator OpenBox()
    {
        StartCoroutine(Shake(box.image.rectTransform));
        if (pickedCards.Length == 0)
            Initialize();

        displayedCardsObjects = new List<GameObject>();
        while (true)
        {
            if (isShakingDone)
            {
                box.image.sprite = boxNextSprite;
                //Display cards
                for (int i = 0; i < pickedCards.Length; i++)
                {
                    GameObject go = Instantiate(displayedCardModel).gameObject;
                    RectTransform rt = go.GetComponent<RectTransform>();
                    go.transform.SetParent(box.transform);
                    rt.anchoredPosition = Vector2.zero;
                    go.GetComponent<Image>().SetNativeSize();
                    go.name = "Picked card " + pickedCards[i].name;

                    rt.localScale = new Vector3(0.16f, 0.16f, 1);
                    System.Type myType = System.Type.GetType(pickedCards[i].GetType() + ",Assembly-CSharp");
                    go.AddComponent(myType);
                    Card c = go.GetComponent<Card>();
                    c.front = pickedCards[i].front;
                    displayedCardsObjects.Add(go);
                }
                StartCoroutine(MoveCards(displayedCardsObjects, 10));
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator Shake(RectTransform item)
    {
        float elapsed = 0.0f;

        Vector3 originalPos = item.anchoredPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / shakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= shakeMagnitude * damper;
            y *= shakeMagnitude * damper;

            item.anchoredPosition = new Vector3(x, y, originalPos.z);

            yield return null;
        }
        item.anchoredPosition = originalPos;
        isShakingDone = true;
    }

    IEnumerator MoveCards(List<GameObject> cards, float speed)
    {
        while (true)
        {
            for (int i = 0; i < cardsFinalPositions.Length; i++)
                if (cards[i] != null)
                    cards[i].GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(cards[i].GetComponent<RectTransform>().anchoredPosition, cardsFinalPositions[i].GetComponent<RectTransform>().anchoredPosition, speed);  // performance beast
            yield return null;
        }
    }

}
