using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class BarrierLineMiddleGame : MonoBehaviour {

    private Image _myImage;
    public Image myImage
    {
        get
        {
            if (_myImage == null)
            {
                _myImage = GetComponent<Image>();
            }
            return _myImage;
        }
    }

    void Start()
    {
        
        transform.parent.GetComponentsInChildren<SpecialBuildingSlot>().ToList().ForEach(s =>
        {
            s.onCardEnterCallback += OnEnterSpecialBuildingSlot;
            s.onCardExitCallback += OnExitSpecialBuildingSlot;
        });
    }

    void OnEnterSpecialBuildingSlot(CardSlot cardSlot, Card card)
    {
        if(card is Barrier){
            myImage.enabled = true;
        }
    }

    void OnExitSpecialBuildingSlot(CardSlot cardSlot, Card card)
    {
        if (cardSlot.myPlayer.myBoard.specialBuildingCardSlotLeft.FindCards<Barrier>().Count == 0 && cardSlot.myPlayer.myBoard.specialBuildingCardSlotRight.FindCards<Barrier>().Count == 0)
        {
            myImage.enabled = false;
        }
        
    }
}
