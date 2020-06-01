using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManagerScript : MonoBehaviour
{
    [SerializeField] private AudioSource thisBg;
    private AudioManagerScript thisAudioManager;

    private float t = 0f;
    private float thisFadeOutDuration = 0.3f;

    [SerializeField] private GameObject thisFujikoDialogBox;
    [SerializeField] private GameObject thisGoemonDialogBox;
    [SerializeField] private Text thisFujikoText;
    [SerializeField] private Text thisGoemonText;
    [SerializeField] private Text thisPressEnter;
    [SerializeField] private GameObject thisFujikoLip;

    private List<string> thisFujikoDialogBook = new List<string>();
    private List<string> thisGoemonDialogBook = new List<string>();

    private bool thisFujikoHasSpoken = false;

    private int thisCurrentDialogIndex = 0;

    private enum State
    {
        eWaiting,
        eFujikoSpeaks,
        eGoemonSpeaks
    }

    private State thisDialogState;

    protected void Awake()
    {
        //thisAudioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        thisAudioManager = (AudioManagerScript)FindObjectOfType(typeof(AudioManagerScript));
    }

    protected void Start()
    {
        thisDialogState = State.eFujikoSpeaks;
        InitializeFujikoDialogBook();
        InitializeGoemonDialogBook();
    }

    protected void InitializeFujikoDialogBook()
    {
        thisFujikoDialogBook.Add("Goemon , you stink !!! Are you okay?");
        thisFujikoDialogBook.Add("What was that? I've stolen too many things , including many men's hearts.");
        thisFujikoDialogBook.Add("Oh , the tub. I had a good time with it eailier today.");
        thisFujikoDialogBook.Add("Hmmmm...How about we play a memory match game?");
        thisFujikoDialogBook.Add("Flip the cards , and match as many pairs as possible to damage my hp.");
        thisFujikoDialogBook.Add("If a pair is matched , you can give me a stupid makeover as you want.");
        thisFujikoDialogBook.Add("If I match a pair, you are gonna look very funny. And you'll shout out stupid lines as well.");
        thisFujikoDialogBook.Add("Wait... I'll let you be the first hand. How bout dat?");
        thisFujikoDialogBook.Add("If you have higher hp at the end , you win.");
        thisFujikoDialogBook.Add("And you get your lovely tub back.");
    }

    protected void InitializeGoemonDialogBook()
    {
        thisGoemonDialogBook.Add("Not okay until I get what you have stolen from me.");
        thisGoemonDialogBook.Add("My favorite cauldron bathtub. I can't sleep without it.");
        thisGoemonDialogBook.Add("I need it , too. Fujiko , let's be reasonable.");
        thisGoemonDialogBook.Add("Sounds stupid , but go ahead and explain.");
        thisGoemonDialogBook.Add("...");
        thisGoemonDialogBook.Add("...I don't think you need a makeover to look stupid.");
        thisGoemonDialogBook.Add("...Oh this is so fair. Not humiliating at all. I'm out.");
        thisGoemonDialogBook.Add("...");
        thisGoemonDialogBook.Add("...");
        thisGoemonDialogBook.Add("Deal. Make sure you disinfect my tub before returning it.");
    }


    protected void Update()
    {
        UpdateStateSwitch();
    }

    protected void UpdateStateSwitch()
    {
        switch (thisDialogState)
        {
            case State.eWaiting:
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    thisPressEnter.gameObject.SetActive(false);

                    if (thisFujikoHasSpoken)
                    {
                        thisDialogState = State.eGoemonSpeaks;
                    }
                    else
                    {
                        // move on to next dialog round
                        thisCurrentDialogIndex += 1;
                        // if out of dialog, start next scene
                        if (thisCurrentDialogIndex >= thisFujikoDialogBook.Count)
                        {
                            thisAudioManager.StartFadingOut("Tutorial", thisBg, thisBg.volume, thisFadeOutDuration, t);
                            // has to add return or the states go on
                            return;
                        }

                        thisDialogState = State.eFujikoSpeaks;
                    }
                }
            }
            break;

            case State.eFujikoSpeaks:
                {
                    HideGoemonDialog();
                    DisplayFujikoDialog();
                    thisFujikoLip.GetComponent<LipMovingScript>().StartTalking();

                    thisFujikoHasSpoken = true;
                    thisDialogState = State.eWaiting;
                }
            break;

            case State.eGoemonSpeaks:
                {
                    HideFujikoDialog();
                    DisplayGoemonDialog();
                    thisFujikoLip.GetComponent<LipMovingScript>().StopTalking();

                    thisFujikoHasSpoken = false;

                    // switch state
                    thisDialogState = State.eWaiting;
                }
            break;
        }
    }

    protected void HideFujikoDialog()
    {
        thisFujikoDialogBox.gameObject.SetActive(false);
        thisFujikoText.text = " ";
    }

    protected void HideGoemonDialog()
    {
        thisGoemonDialogBox.gameObject.SetActive(false);
        thisGoemonText.text = " ";
    }

    protected void DisplayFujikoDialog()
    {
        thisFujikoDialogBox.gameObject.SetActive(true);
        thisFujikoText.text = thisFujikoDialogBook[thisCurrentDialogIndex];
    }

    protected void DisplayGoemonDialog()
    {
        thisGoemonDialogBox.gameObject.SetActive(true);
        thisGoemonText.text = thisGoemonDialogBook[thisCurrentDialogIndex];
    }
}
