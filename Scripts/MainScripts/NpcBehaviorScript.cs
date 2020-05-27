using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class NpcBehaviorScript : MonoBehaviour
{
    // player
    [SerializeField] private GameObject thePlayer;
    private PlayerBehaviorScript thisPlayerBehavior;

    // UI manager
    [SerializeField] private GameObject theUiManager;
    private UiManagerScript thisUiManager;

    // Card Server
    [SerializeField] private GameObject theCardServer;
    private CardServerScript thisCardServer;

    // a list to store npc's memory. max length <= 2
    private List<GameObject> thisMemoryList = new List<GameObject>();

    // a list to store npc's selection. max length <= 2
    private List<GameObject> thisSelectionList = new List<GameObject>();

    // a list that contains all the served cards, including flipped and not flipped ones
    private List<GameObject> thisAllCardDeck = new List<GameObject>();

    // clickable cards will be stored here
    public List<GameObject> thisClickableCards = new List<GameObject>();

    // the time duration that two cards stay flipped once clicked
    private float thisCardDisplayDuration = 1.3f;

    // npc action is automated so it needs action lag to make it more visible
    private float thisActionLag = 2f;

    // a timer to keep track of time
    private float thisStateTimer;

    // npc hp
    private int thisHitPoints = 18;

    // this is a gate to control whether npc can have another consecutive round
    private bool thisOneMoreRoundAllowed = false;

    // this is a gate to make the player state to wait for the lerping animation
    private bool thisIsNowLerpingToShuffle = false;

    private bool thisGameHasEnded = false;

    private enum State
    {
        eActivated,
        eZeroSelected,
        eOneSelected,
        eTwoSelected,
        eWaitDuringLerping,
        eIdle
    }

    private State thisNpcState;

    protected void Start()
    {
        InitializeNpcState();
        InitializePlayerAsOpponent();
        InitializeUiManager();
        InitializeCardServer();
        InitializeAllCardDeck();
        InitializeStateTimer();
    }

    protected void InitializeNpcState()
    {
        thisNpcState = State.eIdle;
    }

    protected void InitializePlayerAsOpponent()
    {
        thisPlayerBehavior = thePlayer.GetComponent<PlayerBehaviorScript>();
    }

    protected void InitializeUiManager()
    {
        thisUiManager = theUiManager.GetComponent<UiManagerScript>();
    }

    protected void InitializeStateTimer()
    {
        /* this func initializes the time by aligning it to action lag duration,
         * which makes the npc slowly flips the card so that player can take a good look. */
        thisStateTimer = thisActionLag;
    }

    protected void InitializeStateTimerToDisplayDuration()
    {
        /*this is used for card display time. 
         * After this much time, the card will flip back.*/
        thisStateTimer = thisCardDisplayDuration;
    }

    protected void InitializeCardServer()
    {
        thisCardServer = theCardServer.GetComponent<CardServerScript>();
    }

    protected void InitializeAllCardDeck()
    {
        List<GameObject> thoseServedCards = thisCardServer.GetServedCards();

        foreach (GameObject aCard in thoseServedCards)
        {
            thisAllCardDeck.Add(aCard);
        }
    }


    protected void Update()
    {
        UpdateStateSwitch();
    }

    protected void UpdateStateSwitch()
    {
        switch (thisNpcState)
        {
            case State.eIdle:
                {

                }
                break;

            case State.eActivated:
                {
                    /* this state is an interrim wake state 
                     * that completes some one-time things before selection*/

                    //thisUiManager.StopAnySlash(thisPlayerBehavior.gameObject);

                    thisUiManager.ToggleAllCardsEmittable(false);

                    // once activated, update clickable list first
                    UpdateClickableList();

                    // prevent player from having another turn
                    thisOneMoreRoundAllowed = false;
                    thisNpcState = State.eZeroSelected;
                }
                break;

            case State.eZeroSelected:
                {
                    if (!thisGameHasEnded)
                    {
                        thisStateTimer -= Time.deltaTime;

                        if (thisStateTimer < 0)
                        {
                            RandomlyFlipOneCard();
                            InitializeStateTimer();
                            thisNpcState = State.eOneSelected;

                            return;
                        }

                        if (thisStateTimer < thisActionLag / 2)
                        {
                            // during half of the action lag, update the turn prompt
                            thisUiManager.UpdateNpcTurn();

                            // makes all cards emittable when cursor hovered on cards
                            //thisUiManager.MakeAllCardsEmittable();

                            return;
                        }
                    }
                }
                break;

            case State.eOneSelected:
                {
                    thisStateTimer -= Time.deltaTime;

                    if (thisStateTimer < 0)
                    {
                        // check if cards in memory are flippable
                        // note: current this assumes that there must be two cards in memory, not considering time countdown
                        // this also assumes that the two in memory should be the same in flippability
                        // note: the public method returns if the card has been flipped; true means it is not flippable
                        bool oneFlipped = thisMemoryList[0].GetComponent<CardScript>().GetIfCardHasBeenFlipped();
                        bool anotherFlipped = thisMemoryList[1].GetComponent<CardScript>().GetIfCardHasBeenFlipped();

                        // if they are both flippable, they must be different from each other
                        if (!oneFlipped && !anotherFlipped)
                        {
                            if (CheckIfTwoTilesAreTheSame(thisMemoryList[0], thisSelectionList[0]))
                            {
                                // if the first one in memory matches the already selected one
                                // add the one in the memory as the second selected one so that they can match
                                FlipTheParticularCard(thisMemoryList[0]);
                                //print("Npc flipped the matched one" + thisMemoryList[0].name +" \n");

                                // reset timer
                                InitializeStateTimerToDisplayDuration();

                                UpdateMemoryFromSelfSelection();
                                // switch to next state
                                thisNpcState = State.eTwoSelected;
                            }
                            else if (CheckIfTwoTilesAreTheSame(thisMemoryList[1], thisSelectionList[0]))
                            {
                                FlipTheParticularCard(thisMemoryList[1]);
                                //print("Npc flipped the matched one" + thisMemoryList[1].name + " \n");
                                InitializeStateTimerToDisplayDuration();
                                UpdateMemoryFromSelfSelection();
                                thisNpcState = State.eTwoSelected;
                            }
                            else
                            {
                                RandomlyFlipOneCard();
                                //print("Npc flipped another random one\n");
                                InitializeStateTimerToDisplayDuration();
                                UpdateMemoryFromSelfSelection();
                                thisNpcState = State.eTwoSelected;
                            }
                        }
                        else
                        {
                            RandomlyFlipOneCard();
                            //print("Npc flipped another random one\n");
                            InitializeStateTimerToDisplayDuration();
                            UpdateMemoryFromSelfSelection();
                            thisNpcState = State.eTwoSelected;
                        }
                    }

                }
                break;

            case State.eTwoSelected:
                {
                    // when timer goes off, cards will be flipped back
                    thisStateTimer -= Time.deltaTime;

                    if (thisStateTimer < 0)
                    {
                        // check pairs
                        CheckSelectedPairs();

                        ClearSelection();

                        if (thisOneMoreRoundAllowed)
                        {
                            StartNpcTurn();
                            InitializeStateTimer();
                            return;
                        }
                        else
                        {
                            if (thisIsNowLerpingToShuffle)
                            {
                                thisNpcState = State.eWaitDuringLerping;
                                // make prompt to indicate shuffling
                                thisUiManager.UpdateShuffling();
                                return;
                            }
                            else
                            {
                                // makes all cards emittable when cursor hovered on cards
                                thisUiManager.ToggleAllCardsEmittable(true);

                                thisNpcState = State.eIdle;
                                thisPlayerBehavior.StartPlayerTurn();
                                InitializeStateTimer();
                            }
                        }
                    }
                }
                break;

            case State.eWaitDuringLerping:
                {
                    if (!thisIsNowLerpingToShuffle)
                    {
                        // makes all cards emittable when cursor hovered on cards
                        thisUiManager.ToggleAllCardsEmittable(true);

                        thisNpcState = State.eIdle;
                        thisPlayerBehavior.StartPlayerTurn();
                        InitializeStateTimer();
                    }
                }
                break;
        }
    }

    protected void ClearSelection()
    {
        thisSelectionList = new List<GameObject>();
    }

    protected bool CheckIfTwoTilesAreTheSame(GameObject aCard, GameObject anotherCard)
    {

        // get the card front of the prefab gameobject of each
        GameObject oneCardFront = aCard.transform.GetChild(1).gameObject;
        GameObject anotherCardFront = anotherCard.transform.GetChild(1).gameObject;

        // get the sprite of the card front
        Sprite oneSprite = oneCardFront.GetComponent<SpriteRenderer>().sprite;
        Sprite anotherSprite = anotherCardFront.GetComponent<SpriteRenderer>().sprite;

        if (oneSprite == anotherSprite)
        {
            return true;
        }
        return false;
    }

    protected void UpdateClickableList()
    {
        /* this func loops thru all the cards on the desktop
         * and checks if teh card is in memory or is flipped
         * if not for both, then it is a clickable card.
         * clickable here means clickable and actionable for npc
         */

        // clear the list first
        thisClickableCards = new List<GameObject>();
        
        // outer loop: all cards on the desk
        foreach (GameObject aCard in thisAllCardDeck)
        {
            bool theCardHasBeenFlipped = aCard.GetComponent<CardScript>().GetIfCardHasBeenFlipped();

            // if the card has not been flipped
            if (!theCardHasBeenFlipped)
            {
                // inner loop: all cards in the memory
                if (!CheckIfTheCardIsInMemory(aCard))
                {
                    thisClickableCards.Add(aCard);
                }
            }
        }

        // game over condition 1
        if (thisClickableCards.Count == 0)
        {
            // when the game is over, temporarily exit out of game editor
            //UnityEditor.EditorApplication.isPlaying = false;
            thisGameHasEnded = true;

            StartCoroutine(CountDownForResult());
            //thisCardServer.AnnounceGameResult();
        }
    }

    protected IEnumerator CountDownForResult()
    {
        // wait for a while to get the result
        yield return new WaitForSeconds(1.5f);

        thisCardServer.AnnounceGameResult();
        yield break;
    }

    protected bool CheckIfTheCardIsInMemory(GameObject aCard)
    {
        /* loop thru the memory
         * compare one card to each card in memory
         * if the card is the same as one of the cards in memory
         * return true
         * if it is none of the ones in the memory
         * return false
         */

        foreach (GameObject aMemorizedCard in thisMemoryList)
        {
            if (TwoCardsAreTheSame(aCard, aMemorizedCard))
            {
                return true;
            }
        }
        return false;
    }

    protected bool TwoCardsAreTheSame(GameObject oneCard, GameObject anotherCard)
    {
        // comparing transform should work 
        // because one card slot won't have over one card
        return (oneCard.transform == anotherCard.transform);
    }

    public void StartNpcTurn()
    {
        if (thisHitPoints > 0)
        {
            thisNpcState = State.eActivated;
        }
        else
        {
            thisNpcState = State.eIdle;
            thisPlayerBehavior.StartPlayerTurn();
        }
    }

    protected void RandomlyFlipOneCard()
    {
        // randomly choose one card from the pool
        GameObject aCard = thisClickableCards[Random.Range(0, thisClickableCards.Count)];
        // remove it to prevent redundant selection
        thisClickableCards.Remove(aCard);
        // add to selection
        thisSelectionList.Add(aCard);
        // start animation
        aCard.GetComponent<CardScript>().StartFlipCoroutine();

        //print("randomly select one card: " + aCard.name + "\n");
    }

    protected void FlipTheParticularCard(GameObject aCard)
    {
        thisSelectionList.Add(aCard);

        aCard.GetComponent<CardScript>().StartFlipCoroutine();
    }

    public void UpdateMemoryFromPlayerSelection(GameObject theCardSelectedByPlayer)
    {
        /* this func can be called by both the player and the npc itself
         * because no matter who it is
         * once a card is flipped
         * the npc remebers it and forget the more distant one
         */

        int theMemoryLength = thisMemoryList.Count;

        switch (theMemoryLength)
        {
            // case 0 and 1 is the same; just add it in
            case (0):
            case (1):
                {
                    thisMemoryList.Add(theCardSelectedByPlayer);
                }
                break;

            case (2):
                {
                    // forget the former, add the latest to the tail
                    thisMemoryList.RemoveAt(0);
                    thisMemoryList.Add(theCardSelectedByPlayer);
                }
                break;
        }
    }

    protected void UpdateMemoryFromSelfSelection()
    {
        // make sure there are two already in the memory
        Assert.AreEqual(2, thisMemoryList.Count);

        // clear the memory to avoid pointer problems
        thisMemoryList = new List<GameObject>();

        // add the ones that just got sleected
        thisMemoryList.Add(thisSelectionList[0]);
        thisMemoryList.Add(thisSelectionList[1]);

    }

    public int GetNpcHitPoints()
    {
        return thisHitPoints;
    }

    public void OnNormalDamage()
    {
        thisHitPoints -= 3;
    }

    public void OnDoubleDamage()
    {
        thisHitPoints -= 6;
    }

    protected void CheckSelectedPairs()
    {
        // get the prefab gameobject respectively from player's selection
        GameObject oneCardPrefab = thisSelectionList[0];
        GameObject anotherCardPrefab = thisSelectionList[1];

        // get the card front of the prefab gameobject of each
        GameObject oneCardFront = oneCardPrefab.transform.GetChild(1).gameObject;
        GameObject anotherCardFront = anotherCardPrefab.transform.GetChild(1).gameObject;

        // get the sprite of the card front
        Sprite oneSprite = oneCardFront.GetComponent<SpriteRenderer>().sprite;
        Sprite anotherSprite = anotherCardFront.GetComponent<SpriteRenderer>().sprite;


        // if having same card front patterns
        if (oneSprite == anotherSprite)
        {
            // start humiliation
            thisUiManager.StartPlayerHumiliation();

            // and do according extra things based on the special tiles
            string theCardCategory = oneCardPrefab.tag;

            switch (theCardCategory)
            {
                case "Normal":
                    {
                        HurtPlayer();
                    }
                    break;

                case "Flower":
                    {
                        HurtPlayerByDouble();
                    }
                    break;

                case "Bird":
                    {
                        HurtPlayer();
                        // npc gets to have one more turn
                        //print("one more turn for Npc\n");
                        thisOneMoreRoundAllowed = true;
                    }
                    break;

                case "Moon":
                    {
                        OnHeal();
                    }
                    break;

                case "Wind":
                    {
                        HurtPlayer();
                        // set lerping to true before card server checks the card
                        // in case the wind is the last pair and the isLerping needs to be closed
                        thisIsNowLerpingToShuffle = true;
                        thisCardServer.ShuffleClickableCards();
                        
                    }
                    break;
            }
        }
        else
        {
            CardScript oneCardScript = oneCardPrefab.GetComponent<CardScript>();
            CardScript anotherCardScript = anotherCardPrefab.GetComponent<CardScript>();

            oneCardScript.StartFlipBackCoroutine();
            anotherCardScript.StartFlipBackCoroutine();

            oneCardScript.StopFlipCoroutine();
            anotherCardScript.StopFlipCoroutine();
        }
    }

    public void CloseShuffleState()
    {
        thisIsNowLerpingToShuffle = false;
    }

    public bool GetShuffleState()
    {
        return thisIsNowLerpingToShuffle;
    }

    protected void HurtPlayer()
    {
        thisPlayerBehavior.OnNormalDamage();
        thisUiManager.UpdatePlayerText();
        if (thisPlayerBehavior.GetPlayerHitPoints() > 0)
        {
            thisUiManager.StopAnySlash(thisPlayerBehavior.gameObject);
            thisUiManager.ActivateSingleSlash(thisPlayerBehavior.gameObject);
        }
    }

    protected void HurtPlayerByDouble()
    {
        thisPlayerBehavior.OnDoubleDamage();
        thisUiManager.UpdatePlayerText();
        thisUiManager.StartInGameLine("DoubleDamage", this.gameObject);
        if (thisPlayerBehavior.GetPlayerHitPoints() > 0)
        {
            thisUiManager.StopAnySlash(thisPlayerBehavior.gameObject);
            thisUiManager.ActivateDoubleSlash(thisPlayerBehavior.gameObject);
        }
    }

    protected void OnHeal()
    {
        thisHitPoints += 3;
        thisUiManager.ActivateLeaf(this.gameObject);
        thisUiManager.UpdateNpcText();
        thisUiManager.StartInGameLine("Healed", this.gameObject);
    }
}
