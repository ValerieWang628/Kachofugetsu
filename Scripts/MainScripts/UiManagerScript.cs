using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UiManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject thisCardServer;

    [SerializeField] private Text thisNpcHpText;
    [SerializeField] private Text thisPlayerHpText;
    [SerializeField] private Text thisNpcZenPrompt;
    [SerializeField] private Text thisPlayerZenPrompt;
    [SerializeField] private Text thisTurnPrompt;

    [SerializeField] private GameObject thisPlayerDialogBox;
    [SerializeField] private GameObject thisNpcDialogBox;

    [SerializeField] private GameObject theNpc;
    [SerializeField] private GameObject thePlayer;

    [SerializeField] private GameObject theGoemonEye;
    [SerializeField] private GameObject theFujikoEye;

    private EyeBlinkingScript thisGoemonEye;
    private EyeBlinkingScript thisFujikoEye;

    [SerializeField] private GameObject thisSwirl;
    [SerializeField] private GameObject thisShuffleGlow;

    [SerializeField] private GameObject thisPlayerLeaf;
    [SerializeField] private GameObject thisNpcLeaf;

    private NpcBehaviorScript thisNpcBehavior;
    private PlayerBehaviorScript thisPlayerBehavior;

    [SerializeField] private GameObject thisPlayerSingleSlashDot;
    [SerializeField] private GameObject thisPlayerDoubleSlashdot1;
    [SerializeField] private GameObject thisPlayerDoubleSlashdot2;

    [SerializeField] private GameObject thisNpcSingleSlashDot;
    [SerializeField] private GameObject thisNpcDoubleSlashdot1;
    [SerializeField] private GameObject thisNpcDoubleSlashdot2;

    protected void Start()
    {
        InitializeData();

        UpdateNpcText();
        UpdatePlayerText();
    }

    protected void InitializeData()
    {
        thisNpcBehavior = theNpc.GetComponent<NpcBehaviorScript>();
        thisPlayerBehavior = thePlayer.GetComponent<PlayerBehaviorScript>();

        thisGoemonEye = theGoemonEye.GetComponent<EyeBlinkingScript>();
        thisFujikoEye = theFujikoEye.GetComponent<EyeBlinkingScript>();
    }

    public void UpdateNpcText()
    {
        int npcHp = thisNpcBehavior.GetNpcHitPoints();
        if (npcHp > 0)
        {
            thisNpcHpText.text = npcHp.ToString();
        }
        else
        {
            thisNpcHpText.text = " ";
            thisNpcZenPrompt.gameObject.SetActive(true);
        }
    }

    public void UpdatePlayerText()
    {
        int playerHp = thisPlayerBehavior.GetPlayerHitPoints();
        if (playerHp > 0)
        {
            thisPlayerHpText.text = playerHp.ToString();
        }
        else
        {
            thisPlayerHpText.text = " ";
            thisPlayerZenPrompt.gameObject.SetActive(true);
        }
    }

    public void UpdatePlayerTurn()
    {
        thisTurnPrompt.text = "Goemon";
    }

    public void StopAnySlash(GameObject theDamagedParty)
    {
        if (theDamagedParty == thisPlayerBehavior.gameObject)
        {
            StopSlash(thisPlayerSingleSlashDot);
            StopSlash(thisPlayerDoubleSlashdot1);
            StopSlash(thisPlayerDoubleSlashdot2);
        }
        else if (theDamagedParty == thisNpcBehavior.gameObject)
        {
            StopSlash(thisNpcSingleSlashDot);
            StopSlash(thisNpcDoubleSlashdot1);
            StopSlash(thisNpcDoubleSlashdot2);
        }
    }

    protected void StopSlash(GameObject theSlash)
    {
        SlashScript theSlashScript = theSlash.GetComponent<SlashScript>();

        if (theSlashScript.GetCoroutineStatus())
        {
            theSlashScript.StopSlash();
            //theSlashScript.ResetPos();
        }
    }

    public void ActivateSingleSlash(GameObject theDamagedParty)
    {
        if (theDamagedParty == thisPlayerBehavior.gameObject)
        {
            thisPlayerSingleSlashDot.GetComponent<SlashScript>().StartSlash();
        }
        else if (theDamagedParty == thisNpcBehavior.gameObject)
        {
            thisNpcSingleSlashDot.GetComponent<SlashScript>().StartSlash();
        }
    }

    public void ActivateDoubleSlash(GameObject theDamagedParty)
    {
        if (theDamagedParty == thisPlayerBehavior.gameObject)
        {
            thisPlayerDoubleSlashdot1.GetComponent<SlashScript>().StartSlash();
            thisPlayerDoubleSlashdot2.GetComponent<SlashScript>().StartSlash();
        }
        else if (theDamagedParty == thisNpcBehavior.gameObject)
        {
            thisNpcDoubleSlashdot1.GetComponent<SlashScript>().StartSlash();
            thisNpcDoubleSlashdot2.GetComponent<SlashScript>().StartSlash();
        }
    }

    public void UpdateNpcTurn()
    {
        thisTurnPrompt.text = "Fujiko";
    }

    public void UpdateShuffling()
    {
        thisTurnPrompt.text = "Shuffling";
        ActivateSwirl();
        ActivateShuffleGlow();
    }

    public void ActivateLeaf(GameObject healedSubject)
    {
        switch (healedSubject.tag)
        {
            case "Player":
                {
                    thisPlayerLeaf.SetActive(true);
                }
                break;

            case "Npc":
                {
                    thisNpcLeaf.SetActive(true);
                }
                break;
        }
    }

    protected void ActivateSwirl()
    {
        thisSwirl.GetComponent<SwirlScript>().Manifest();
    }

    protected void ActivateShuffleGlow()
    {
        thisShuffleGlow.GetComponent<ShuffleGlowScript>().StartToFadeInOut();
    }

    public void ToggleAllCardsEmittable(bool allowed)
    {
        List<GameObject> thisAllCards = thisCardServer.GetComponent<CardServerScript>().GetServedCards();
        foreach (GameObject aCard in thisAllCards)
        {
            if (allowed)
            {
                aCard.GetComponent<CardScript>().MakeEmittable();
            }
            else
            {
                aCard.GetComponent<CardScript>().MakeNotEmittable();
            }
            
        }
    }

    public void StartPlayerHumiliation()
    {
        thisGoemonEye.StartHumiliation();
        thisGoemonEye.AbortState();
        thisPlayerDialogBox.SetActive(true);
    }

    public void StartNpcHumiliation()
    {
        thisFujikoEye.StartHumiliation();
        thisFujikoEye.AbortState();
        thisNpcDialogBox.SetActive(true);
    }

}
