using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardServerScript : MonoBehaviour
{

    // player
    [SerializeField] private GameObject thePlayer;
    private PlayerBehaviorScript thisPlayerBehavior;

    // NPC
    [SerializeField] private GameObject theNpc;
    private NpcBehaviorScript thisNpcBehavior;

    // a point where all cards gather, wait to be shuffled
    [SerializeField] private GameObject thisShufflePoint;

    private float thisLerpToShuffleSpeed = 0.1f;

    // a list that stores 9 cards; each card is different from others
    [SerializeField] private List<GameObject> thisCardPrefabList = new List<GameObject>();

    // a list that stores all the dots in the arena --- dots to position the card
    private List<GameObject> thisCardPointList = new List<GameObject>();

    // a list that stores 18 cards that have not been served
    private List<GameObject> thisAllCardDeck = new List<GameObject>();

    // a list that stores 18 cards that have been served
    // so the position of each card is stored here
    private List<GameObject> thisInstantiatedServedCards = new List<GameObject>();

    // a list to store the current position of the cards waiting to be shuffled
    private List<Vector3> thisAllocatedCardPositionList = new List<Vector3>();

    private bool thisHasShuffled = false;

    protected void Awake()
    {
        InitializeCardPoints();
        InitializeAllCardDeck();
        ServeTheCards();
    }

    protected void Start()
    {
        InitializeNpc();
        InitializePlayer();
    }

    protected void InitializeCardPoints()
    {
        /* find all the card points
         * and add them to the list
         */

        // transferring an array to list
        GameObject[] theCardPointsArray = GameObject.FindGameObjectsWithTag("CardPoints");
        thisCardPointList = new List<GameObject>(theCardPointsArray);
    }

    protected void InitializeAllCardDeck()
    {
        /* currently in the prefab list, there are 9 gameobjects
         * this func add the 9 prefabs twice into the AllCardDeck to  make up a 18-long list
         */

        /* adding twice in a loop below might be a dumb approach but it is safe
         * because I don't want to have any weird list-copying referencing issue
         * the reason is: if not deepcopying, there will be an enumeration operation problem.
         */

        foreach (GameObject aPrefab in thisCardPrefabList)
        {
            // add twice
            thisAllCardDeck.Add(aPrefab);
            thisAllCardDeck.Add(aPrefab);
        }
    }

    protected void InitializePlayer()
    {
        thisPlayerBehavior = thePlayer.GetComponent<PlayerBehaviorScript>();
    }

    protected void InitializeNpc()
    {
        thisNpcBehavior = theNpc.GetComponent<NpcBehaviorScript>();
    }

    protected void ServeTheCards()
    {
        /* for each dots in the arena,
         * instantiate a prefab from the 18-list
         * after one instantiation, remove the prefab added
         * so that the prefabs won't repeat when served
         */

        foreach (GameObject aCardPoint in thisCardPointList)
        {
            GameObject aCardPrefab = thisAllCardDeck[Random.Range(0, thisAllCardDeck.Count)];

            thisAllCardDeck.Remove(aCardPrefab);

            Vector3 aCardPointPosition = aCardPoint.transform.position;

            GameObject thisInstantiatedCard = Instantiate(aCardPrefab, aCardPointPosition, Quaternion.identity);

            thisInstantiatedServedCards.Add(thisInstantiatedCard);
        }
    }

    public List<GameObject> GetServedCards()
    {
        return thisInstantiatedServedCards;
    }

    protected void Update()
    {
        // one-time check
        if (thisHasShuffled)
        {
            if (CheckIfShuffleCompleted())
            {
                if (thisPlayerBehavior.GetShuffleState())
                {
                    thisPlayerBehavior.CloseShuffleState();
                }
                // else if npc is shuffling, close npc shuffle state
                else if (thisNpcBehavior.GetShuffleState())
                {
                    thisNpcBehavior.CloseShuffleState();
                }
                thisHasShuffled = false;
            }
        }        
    }

    protected void AllocateCardPosition()
    {
        foreach (GameObject aCard in thisInstantiatedServedCards)
        {
            if (CheckIfClickable(aCard))
            {
                thisAllocatedCardPositionList.Add(aCard.transform.position);
            }
        }
    }

    public void ShuffleClickableCards()
    {   
        AllocateCardPosition();

        // if none of the cards are clickable, don't bother
        if (thisAllocatedCardPositionList.Count != 0)
        {
            thisHasShuffled = true;

            foreach (GameObject aCard in thisInstantiatedServedCards)
            {
                if (CheckIfClickable(aCard))
                {
                    // access card script component for future call
                    CardScript aCardScript = aCard.GetComponent<CardScript>();

                    // randomly find a position from the allocated card position list
                    Vector3 aRandomlyAssignedPosition = thisAllocatedCardPositionList[Random.Range(0, thisAllocatedCardPositionList.Count)];
                    //Vector3 aRandomlyAssignedPosition = thisAllocatedCardPositionList[Random.Range(0, thisAllocatedCardPositionList.Count)].position;

                    // remove this one from list to prevent redundant assignment
                    thisAllocatedCardPositionList.Remove(aRandomlyAssignedPosition);

                    // make coroutine work
                    aCardScript.StartLerpToShuffleCoroutine(thisShufflePoint.transform.position, aRandomlyAssignedPosition, thisLerpToShuffleSpeed);
                }
            }

            // empty the list for future use
            thisAllocatedCardPositionList = new List<Vector3>();
        }
        else
        {
            // when the game is over, temporarily exit out of game editor
            //UnityEditor.EditorApplication.isPlaying = false;

            thisNpcBehavior.CloseShuffleState();
            thisPlayerBehavior.CloseShuffleState();

            StartCoroutine(CountDownForResult());
        }
    }

    public void AnnounceGameResult()
    {
        int theNpcHp = thisNpcBehavior.GetNpcHitPoints();
        int thePlayerHp = thisPlayerBehavior.GetPlayerHitPoints();
        if (theNpcHp - thePlayerHp > 0)
        {
            SceneManager.LoadScene("Fail");
        }
        else if (theNpcHp - thePlayerHp == 0)
        {
            SceneManager.LoadScene("Tie");
        }
        else if (theNpcHp - thePlayerHp < 0)
        {
            SceneManager.LoadScene("Congrat");
        }
    }

    protected IEnumerator CountDownForResult()
    {
        yield return new WaitForSeconds(1.5f);

        AnnounceGameResult();
        yield break;
    }



    protected bool CheckIfClickable(GameObject aCard)
    {
        return !aCard.GetComponent<CardScript>().GetIfCardHasBeenFlipped();
    }

    protected bool CheckIfShuffleCompleted()
    {
        bool theBoolForAll = true;

        foreach (GameObject aCard in thisInstantiatedServedCards)
        {
            if (CheckIfClickable(aCard))
            {
                CardScript aCardScript = aCard.GetComponent<CardScript>();
                if (!aCardScript.GetIfShuffleComplete())
                {
                    theBoolForAll = false;
                }
            }
        }

        if (theBoolForAll)
        {
            return true;
        }
        return false; 
    }

}
