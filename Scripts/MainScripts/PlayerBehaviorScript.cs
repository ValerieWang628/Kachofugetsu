using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviorScript : MonoBehaviour
{
    // NPC
    [SerializeField] private GameObject theNpc;
    private NpcBehaviorScript thisNpcBehavior;

    // Card Server
    [SerializeField] private GameObject theCardServer;
    private CardServerScript thisCardServer;

    // UI manager
    [SerializeField] private GameObject theUiManager;
    private UiManagerScript thisUiManager;

    // this bools locks/unlocks player click action
    private bool thisCanClickCards;

    // a list with max length 2 to store players' selection
    private List<GameObject> thisCardSelection = new List<GameObject>();

    // the time duration that two cards stay flipped once clicked
    private float thisCardDisplayDuration = 1.3f;

    // a timer to keep track of time
    private float thisStateTimer;

    // player hp
    private int thisHitPoints = 18;

    // this is a gate to control whether player can have another consecutive round
    private bool thisOneMoreRoundAllowed = false;

    // this is a gate to make the player state to wait for the lerping animation
    private bool thisIsNowLerpingToShuffle = false;

    // a list that contains all the served cards, including flipped and not flipped ones
    private List<GameObject> thisAllCardDeck = new List<GameObject>();

    private enum State 
    { 
        eActivated,
        eZeroSelected,
        eOneSelected,
        eTwoSelected,
        eWaitDuringLerping,
        eIdle
    }

    private State thisPlayerState;


    protected void Start()
    {
        InitializePlayerState();
        InitializeNpcAsOpponent();
        InitializeUiManager();
        InitializeCardServer();
        InitializeAllCardDeck();
        InitializeStateTimer();
    }

    protected void InitializePlayerState()
    {
        thisPlayerState = State.eActivated;
    }

    protected void InitializeStateTimer()
    {
        thisStateTimer = thisCardDisplayDuration;
    }

    protected void InitializeNpcAsOpponent()
    {
        thisNpcBehavior = theNpc.GetComponent<NpcBehaviorScript>();
    }

    protected void InitializeUiManager()
    {
        thisUiManager = theUiManager.GetComponent<UiManagerScript>();
    }

    protected void InitializeCardServer()
    {
        thisCardServer = theCardServer.GetComponent<CardServerScript>();
    }

    protected void InitializeAllCardDeck()
    {
        /* this func retrieves all the instantiated cards from the card server
         * but card server and player state starts at the same time
         * so the card server has to serve the card before player's start function
         * so use awake() in card server script
         */

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
        switch (thisPlayerState)
        {
            case State.eActivated:
                {
                    /* this state contains one time things before the turn really starts*/ 

                    // check if all cards are flipped
                    CheckClickableCards();

                    // allow player to click
                    thisCanClickCards = true;

                    // prevent player from having one more turn
                    thisOneMoreRoundAllowed = false;

                    thisPlayerState = State.eZeroSelected;
                }
                break;

            // eZero and eOne are supposed to be empty because they just wait
            case State.eZeroSelected:
                {

                }
                break;

            case State.eOneSelected:
                {

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

                        // empty selection
                        ClearSelection();

                        // reset time for display
                        InitializeStateTimer();

                        if (thisOneMoreRoundAllowed)
                        {
                            // player turn start again
                            StartPlayerTurn();
                            //print("player now has one more round\n");
                            return;
                        }
                        else
                        {
                            if (thisIsNowLerpingToShuffle)
                            {
                                thisUiManager.UpdateShuffling();
                                thisPlayerState = State.eWaitDuringLerping;
                                return;
                            }
                            else
                            {
                                // switch to idle state because the npc's turn is about to start
                                thisPlayerState = State.eIdle;
                                //print("player is now idle\n");
                                thisNpcBehavior.StartNpcTurn();
                            }
                        }
                    }
                }
                break;

            case State.eWaitDuringLerping:
                {
                    // wait for the lerping to end, and switch state
                    if (!thisIsNowLerpingToShuffle)
                    {
                        thisPlayerState = State.eIdle;
                        thisNpcBehavior.StartNpcTurn();
                    }
                }
                break;

            case State.eIdle:
                {
                    // doing nothing
                }
                break;
        }
    }

    protected void CheckClickableCards()
    {
        int clickableCount = 0;

        foreach (GameObject aCard in thisAllCardDeck)
        {
            bool hasBeenFlipped = aCard.GetComponent<CardScript>().GetIfCardHasBeenFlipped();
            if (!hasBeenFlipped)
            {
                clickableCount += 1;
            }
        }

        if (clickableCount == 0)
        {
            // UnityEditor.EditorApplication.isPlaying = false;
            //thisCardServer.AnnounceGameResult();
            StartCoroutine(CountDownForResult());
        }
    }

    protected IEnumerator CountDownForResult()
    {
        yield return new WaitForSeconds(1.5f);

        thisCardServer.AnnounceGameResult();
        yield break;
    }

    public void StartPlayerTurn()
    {
        thisPlayerState = State.eActivated;
        thisUiManager.UpdatePlayerTurn();
    }

    public bool GetIfThePlayerCanClick()
    {
        return thisCanClickCards;
    }

    protected void ClearSelection()
    {
        thisCardSelection = new List<GameObject>();
    }

    public int GetPlayerHitPoints()
    {
        return thisHitPoints;
    }

    public void OnCardSelected(GameObject aCard)
    {
        thisCardSelection.Add(aCard);

        // one card selected
        if (thisCardSelection.Count == 1)
        {
            // update state
            thisPlayerState = State.eOneSelected;
        }
        else 
        if (thisCardSelection.Count == 2)
        {
            // lock player action 
            thisCanClickCards = false;

            // update state
            thisPlayerState = State.eTwoSelected;
        }

        // the npc also sees what is flipped
        thisNpcBehavior.UpdateMemoryFromPlayerSelection(aCard);
    }

    protected void CheckSelectedPairs()
    {
        // get the prefab gameobject respectively from player's selection
        GameObject oneCardPrefab = thisCardSelection[0];
        GameObject anotherCardPrefab = thisCardSelection[1];

        // get the card front of the prefab gameobject of each
        GameObject oneCardFront = oneCardPrefab.transform.GetChild(1).gameObject;
        GameObject anotherCardFront = anotherCardPrefab.transform.GetChild(1).gameObject;

        // get the sprite of the card front
        Sprite oneSprite = oneCardFront.GetComponent<SpriteRenderer>().sprite;
        Sprite anotherSprite = anotherCardFront.GetComponent<SpriteRenderer>().sprite;


        // if having same card front patterns
        if (oneSprite == anotherSprite)
        {


            // and do according extra things based on the special tiles
            string theCardCategory = oneCardPrefab.tag;

            switch (theCardCategory)
            {
                case "Normal":
                    {
                        HurtNpc();
                        return;
                    }

                case "Flower":
                    {
                        // double damage
                        HurtNpcByDouble();
                        return;
                    }

                case "Bird":
                    {
                        HurtNpc();
                        // player gets to have one more turn
                        print("One more turn for player!\n");
                        thisOneMoreRoundAllowed = true;
                        return;
                    }

                case "Moon":
                    {
                        OnHeal();
                        return;
                    }

                case "Wind":
                    {
                        HurtNpc();
                        thisCardServer.ShuffleClickableCards();
                        thisIsNowLerpingToShuffle = true;
                        return;
                    }
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

    protected void HurtNpc()
    {
        // hit Npc, and update the stats
        thisNpcBehavior.OnNormalDamage();
        thisUiManager.UpdateNpcText();
    }

    protected void HurtNpcByDouble()
    {
        thisNpcBehavior.OnDoubleDamage();
        thisUiManager.UpdateNpcText();
    }

    protected void OnHeal()
    {
        thisHitPoints += 3;
        thisUiManager.UpdatePlayerText();
    }

    public void OnNormalDamage()
    {
        thisHitPoints -= 3;
    }

    public void OnDoubleDamage()
    {
        thisHitPoints -= 6;
    }
}
