using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
/// <summary>
/// interface for network and AI take actions on opponent player
/// </summary>
public class PlayerInputControl : MonoBehaviour {

    public Player myPlayer;

    public GameObject myPanel;

    private static PlayerInputControl _instance;

    /// <summary>
    /// Singleton
    /// </summary>
    public static PlayerInputControl instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerInputControl>();
            }
            return _instance;
        }
    }

    public bool showPanelOnStartTurn = false;

    List<System.Action> _allActions;
    public List<System.Action> allActions
    {
        get
        {
            if (_allActions == null)
            {
                _allActions = new List<System.Action>();

                _allActions.Add(EndTurn);

                _allActions.Add(UseWatchTower);
                _allActions.Add(UseWoodenWall);

                _allActions.Add(UseCamp);
                _allActions.Add(UseSwordForge);
                _allActions.Add(UseArmorForge);
                _allActions.Add(UseStrongHold);
                _allActions.Add(UseStorage);
                _allActions.Add(UseTavern);

                _allActions.Add(UseBasicAttack);
                _allActions.Add(UseRaidAttack);
                _allActions.Add(UseRaidFlipped);
                _allActions.Add(UseFullScaleAttack);

                _allActions.Add(UseBasicDefense);
                _allActions.Add(UseCounterAttack);

                _allActions.Add(UseInsight);
                _allActions.Add(UseInsightFlipped);
                _allActions.Add(UseDuel);
                _allActions.Add(UseDuelFlipped);
                _allActions.Add(UseSupply);

                _allActions.Add(UseHelperFighter);
                _allActions.Add(UseHelperFighterFlipped);
                _allActions.Add(UseHelperVanguard);
                _allActions.Add(UseHelperVanguardFlipped);
                _allActions.Add(UseHelperSupplier);
                _allActions.Add(UseHelperSupplierFlipped);

                _allActions.Add(UseChargeAttack);
                _allActions.Add(UseProvision);
                _allActions.Add(UseProvisionFlipped);
                _allActions.Add(UseBash);
                _allActions.Add(UseMeal);
                _allActions.Add(UseMealFlipped);
                _allActions.Add(UseQuarrying);
                _allActions.Add(UseQuarryingFlipped);
            }

            return _allActions;
        }
    }

    void Start()
    {
        myPanel.SetActive(false);
        if (PhotonNetwork.offlineMode && WifiNetworkManager.instance == null)
        {
            GameMechanics.instance.turnManager.onEndTurn += OnTurnEnd;
            GameMechanics.instance.turnManager.onBeginTurn += OnTurnBegin;
            myPlayer.myBoard.myHand.onChangeCardNumber += OnHandChange;

        }
    }

    void OnTurnBegin(Player player)
    {
        if (!showPanelOnStartTurn)
        {
            return;
        }

        if (player == myPlayer)
        {
            myPanel.SetActive(true);

        }
    }

    void OnTurnEnd(Player player)
    {

        if (player == myPlayer)
        {
            myPanel.SetActive(false);
        }
    }

    void OnHandChange(CardSlot cardSlot)
    {
        if (!showPanelOnStartTurn)
        {
            return;
        }
        HighlightAvaiableOptions();

    }



    public List<System.Action> GetAvaiableActions()
    {
        List<System.Action> avaiableActions = new List<System.Action>();

        myPlayer.myBoard.myHand.cards.ForEach(cardHand =>
        {
            bool allowedCard = CheckCardAvaiability(cardHand, true);

            if (!allowedCard)
            {
                allowedCard = CheckCardAvaiability(cardHand, false);
            }

            if (allowedCard)
            {
                Debug.Log("allowed card cardHand " + cardHand.title);
                var actionDelegate = allActions.Find(a => a.Method.Name.Contains(cardHand.title));
                avaiableActions.Add(actionDelegate);
            }
        });

        return avaiableActions;
    }

    void HighlightAvaiableOptions()
    {
        System.Collections.Generic.List<Button> buttons = GetComponentsInChildren<Button>(true).ToList();

        buttons.ForEach(button => button.interactable = false);

        //set actions alwways avaiable
        buttons.ForEach(button =>
        {

            if (button.gameObject.name == "EndTurn" || button.gameObject.name == "ToggleShowCards" || button.gameObject.name == "TogglePanelControl")
            {
                //  Debug.Log("hilight="  + button.gameObject.name);
                button.interactable = true;
            }

        });
        //check hands for avaiable cards
        foreach (var cardHand in myPlayer.myBoard.myHand.cards)
        {

            buttons.ForEach(button =>
            {
                if (button.gameObject.name.Contains(cardHand.title))
                {
                    // Debug.Log("hilight=" + button.gameObject.name);
                    button.interactable = true;
                    if (button.gameObject.name == "SelectWatchTowerRaidBasicAttackFullScaleAttack")
                    {
                        button.interactable = myPlayer.otherPlayer.myBoard.TryGetCardOnField<WatchTower>() != null;
                    }
                }
            });
        }
    }


    #region Special Buildindgs

    public void UseWatchTower()
    {

        UseBuildingCard<WatchTower>();
    }

    public void UseWoodenWall()
    {
        UseBuildingCard<Barrier>();
    }

    #endregion


    #region Normal Buildings

    public void UseSwordForge()
    {
        UseBuildingCard<SwordForge>();
    }

    public void UseArmorForge()
    {
        UseBuildingCard<ArmourForge>();
    }

    public void UseCamp()
    {
        UseBuildingCard<GuestRoom>();

    }

    public void UseTavern()
    {

        UseBuildingCard<StrategyAnalysisLab>();
    }

    public void UseStrongHold()
    {
        UseBuildingCard<DamageAnalysisLab>();
    }

    public void UseStorage()
    {
        UseBuildingCard<AdvancedSupplyBase>();
    }

    #endregion

    #region Aux Building Card Usage

    void UseBuildingCard<T>() where T : Card
    {
        BuildingCard buildingCard = myPlayer.myBoard.myHand.cards.Find(c => c is T) as BuildingCard;
        if (buildingCard == null)
        {
            Debug.Log("UseBuildingCard: No card in Hands " + typeof(T));
            return;
        }

        List<Card> cardsToPay = CardAI.SelectCardsFromHandToPayForBuilding(buildingCard.cost, buildingCard.myPlayer);

        UseBuildingCard(buildingCard, cardsToPay);
    }

    public void UseBuildingCard(BuildingCard buildingCard, List<Card> cardsToPay)
    {
        if (!buildingCard.SelectCardsToPay(cardsToPay.ToArray()) || !buildingCard.IsAllowedToActivate())
        {
            Debug.Log("PLayerInputControl: card " + buildingCard.title + " not allowed");
            return;
        }

        buildingCard.Activate();
    }

    #endregion


    #region Helper Cards
    public void UseHelperFighter()
    {

        TryUseCard<Fighter>(flipped: false);
    }

    public void UseHelperVanguard()
    {
        TryUseCard<Vanguard>(flipped: false);
    }

    public void UseHelperSupplier()
    {
        TryUseCard<Supplier>(flipped: false);
    }



    public void UseHelperFighterFlipped()
    {
        TryUseCard<Fighter>(flipped: true);
    }

    public void UseHelperVanguardFlipped()
    {
        TryUseCard<Vanguard>(flipped: true);
    }

    public void UseHelperSupplierFlipped()
    {
        TryUseCard<Supplier>(flipped: true);
    }

    #endregion


    #region Use Effects 

    public void UseQuarrying()
    {
        EffectCard effectCard = myPlayer.myBoard.myHand.cards.Find(c => c is Quarrying) as EffectCard;
        UsingQuarrying(effectCard);
    }

    public void UsingQuarrying(EffectCard effectCard)
    {
        if (effectCard == null)
        {
            Debug.Log("UseEffectCard: No card in Hands " + typeof(Quarrying));
            return;
        }

        List<Card> cardsToPay = new List<Card>();
        List<Card> myHand = new List<Card>(myPlayer.myBoard.myHand.cards);
        myHand.Remove(effectCard);

        if (myHand.Count < effectCard.cost)
        {
            Debug.Log("UseeffectCard: No enough  cards for cost " + typeof(Quarrying), this);
            return;
        }
        myHand.Shuffle();

        for (int i = 0; i < effectCard.cost; i++)
            cardsToPay.Add(myHand[i]);

        if (!effectCard.SelectCardsToPay(cardsToPay.ToArray()) || !effectCard.IsAllowedToActivate())
        {
            Debug.Log("PLayerInputControl: card " + effectCard.title + " not allowed");
            return;
        }
        effectCard.myCardGraphics.ShowFront(true);
        effectCard.Activate();
    }

    public IEnumerator UseQuarryingFlippedPostattack()
    {
        EffectCard effectCard = myPlayer.myBoard.flippedCardSlot.cards.Find(c => c is Quarrying) as EffectCard;
        UsingQuarrying(effectCard);
        yield return null;
    }

    public void UseQuarryingFlipped()
    {
        Quarrying effectCard = myPlayer.myBoard.myHand.cards.Find(c => c is Quarrying) as Quarrying;
        if (effectCard == null)
        {
            Debug.Log("UseEffectCard: No card in Hands " + typeof(Quarrying));
            return;
        }
        effectCard.myCardGraphics.ShowBack(true);
        effectCard.Activate();
    }

    public void UseMeal()
    {
        TryUseCard<Meal>(flipped: false);
    }

    public void UseMealFlipped()
    {
        TryUseCard<Meal>(flipped: true);
    }

    public void UseProvision()
    {
        TryUseCard<Provision>(flipped: false);
    }

    public void UseProvisionFlipped()
    {
        TryUseCard<Provision>(flipped: true);
    }

    public void UseInsightFlipped()
    {

        TryUseCard<Insight>(flipped: true);
    }

    public void UseInsight()
    {
        TryUseCard<Insight>(flipped: false);
    }
    public void UseDuel(Satisfaction satisfactionCard, HelperCard opponentHelperCard, HelperCard choosedAlready = null)
    {

        if (satisfactionCard == null)
        {
            Debug.Log("UseSatisfaction failed, no satisfaction on hands");
            return;
        }
        if (opponentHelperCard == null)
        {
            Debug.Log("UseSatisfaction failed to get opponent helper card");
            return;
        }
        if (satisfactionCard.myPlayer != myPlayer)
        {
            Debug.Log("_______ UseSatisfaction card is not my card, cant use!");
            return;
        }

        if (opponentHelperCard.myPlayer == myPlayer)
        {
            Debug.Log("_______ UseSatisfaction card target is my card, cant use!");
            return;

        }

        if (!satisfactionCard.IsAllowedToActivate())
        {
            Debug.Log("Cant use " + satisfactionCard.title + " not allowed to activate");
            return;
        }
        satisfactionCard.OnChooseHelperCard(opponentHelperCard);
        satisfactionCard.alreadyChoosedCardToPay = choosedAlready;
        PlayerInputControl.instance.TryUseCard(satisfactionCard, flipped: false);
    }
    public void UseDuel()
    {
        Satisfaction satisfactionCard = myPlayer.myBoard.myHand.cards.Find(c => c is Satisfaction) as Satisfaction;
        if (satisfactionCard == null)
        {
            Debug.Log("UseSatisfaction failed, no satisfaction on hands");
            return;
        }
        HelperCard opponentHelperCard = myPlayer.otherPlayer.myBoard.TryGetCardOnField<HelperCard>();
        if (opponentHelperCard == null)
        {
            Debug.Log("UseSatisfaction failed to get opponent helper card");
            return;
        }
        satisfactionCard.OnChooseHelperCard(opponentHelperCard);
        satisfactionCard.myCardGraphics.ShowFront(true);
        if (satisfactionCard.IsAllowedToActivate())
        {
            satisfactionCard.Activate();
        }
        else
        {
            satisfactionCard.myCardGraphics.ShowBack(true);
        }

    }

    public void UseDuelFlipped()
    {
        Satisfaction satisfactionCard = myPlayer.myBoard.myHand.FindCardSpecefic<Satisfaction>();
        if (satisfactionCard != null)
        {
            HelperCard opponentHelperCard = myPlayer.otherPlayer.myBoard.TryGetCardOnField<HelperCard>();
            if (opponentHelperCard == null)
            {
                Debug.Log("UseSatisfaction failed to get opponent helper card");
                return;
            }
            satisfactionCard.OnChooseHelperCard(opponentHelperCard);

            satisfactionCard.myCardGraphics.ShowBack(true);
            if (satisfactionCard.IsAllowedToActivate())
            {
                satisfactionCard.Activate();
            }

        }

    }

    public void UseSupply()
    {
        TryUseCard<Supply>(flipped: true);

    }

    #endregion


    #region Use Defense Cards

    public void UseBash()
    {
        TryUseCard<Bash>(flipped: true);
    }

    public void UseCounterAttack()
    {
        TryUseCard<CounterAttack>(flipped: true);
    }

    public void UseBasicDefense()
    {
        TryUseCard<BasicDefense>(flipped: true);
    }

    #endregion

    #region Use Attack ACards

    public void TargetWatchTowerForAttack()
    {
        //selfplayer is other player in this case: playerinputcontrol always is opponent

        WatchTower watchTower = GameMechanics.instance.selfPlayer.myBoard.TryGetCardOnField<WatchTower>();
        if (watchTower == null)
        {
            Debug.Log("No watch Tower to choose");
        }
        else
        {
            Debug.Log("Targeting watch Tower ");
            if (watchTower.cardSlot is SpecialBuildingSlot1)
            {
                GameMechanics.instance.selfPlayer.myBoard.attackWaitListSlot.isTargetToWatchTowerLeft = true;
            }
            else if (watchTower.cardSlot is SpecialBuildingSlot2)
            {
                GameMechanics.instance.selfPlayer.myBoard.attackWaitListSlot.isTargetToWatchTowerRight = true;
            }

        }
    }
    public void UseFullScaleAttack()
    {
        TryUseCard<FullScaleAttack>(flipped: false);
    }

    public void UseBasicAttack()
    {
        TryUseCard<BasicAttack>(flipped: false);
    }

    public void UseRaidAttack()
    {
        TryUseCard<Raid>(flipped: false);
    }

    public void UseRaidFlipped()
    {
        TryUseCard<Raid>(flipped: true);
    }

    public void UseChargeAttack()
    {
        TryUseCard<Charge>(flipped: false);
    }

    #endregion 

    /// <summary>
    /// check if is allowed flipping appriate way
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flipped"></param>
    void TryUseCard<T>(bool flipped) where T : Card
    {
        T succed = myPlayer.myBoard.myHand.FindCardSpecefic<T>();
        if (succed != null)
        {
            TryUseCard(succed, flipped);

        }
        else
        {
            Debug.Log("PLayerInputControl: " + typeof(T) + " not found");
        }
    }

    public bool TryUseCard(Card card, bool flipped)
    {
        bool itWasDifferent = (flipped) ? card.myCardGraphics.isShowingFront : card.myCardGraphics.isShowingBack;

        if (flipped)
            card.myCardGraphics.ShowBack(true);
        else
            card.myCardGraphics.ShowFront(true);

        if (card.IsAllowedToActivate())
        {

            card.Activate();
            return true;
        }
        else
        {

            if (itWasDifferent)
            {
                if (flipped)
                    card.myCardGraphics.ShowFront(true);
                else
                    card.myCardGraphics.ShowBack(true);
            }

            Debug.Log("Cant use " + card.title + " not allowed to activate");
            return false;
        }
    }

    public bool CheckCardAvaiability(Card card, bool flipped)
    {
        bool itWasDifferent = (flipped) ? card.myCardGraphics.isShowingFront : card.myCardGraphics.isShowingBack;

        if (flipped)
            card.myCardGraphics.ShowBack(true);
        else
            card.myCardGraphics.ShowFront(true);

        bool allowed = card.IsAllowedToActivate();

        if (itWasDifferent)
        {
            if (flipped)
                card.myCardGraphics.ShowFront(true);
            else
                card.myCardGraphics.ShowBack(true);
        }
        return allowed;
    }

    public void EndTurn()
    {
        GameMechanics.instance.turnManager.EndTurn(myPlayer);
    }


}
