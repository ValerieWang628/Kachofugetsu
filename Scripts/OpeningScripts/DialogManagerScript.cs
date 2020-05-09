﻿using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject thisFujikoDialogBox;
    [SerializeField] private GameObject thisGoemonDialogBox;
    [SerializeField] private Text thisFujikoText;
    [SerializeField] private Text thisGoemonText;
    [SerializeField] private Text thisFujikoPrompt;
    [SerializeField] private Text thisGoemonPrompt;

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

    protected void Start()
    {
        thisDialogState = State.eFujikoSpeaks;
        InitializeFujikoDialogBook();
        InitializeGoemonDialogBook();
    }

    protected void InitializeFujikoDialogBook()
    {
        thisFujikoDialogBook.Add("Goemon, you stink a bit. You doing all great?");
        thisFujikoDialogBook.Add("What was that? I've stolen too many things.");
        thisFujikoDialogBook.Add("Oh, the tub. I had a good time with it eailier today.");
        thisFujikoDialogBook.Add("How about we play a memory-match game?");
        thisFujikoDialogBook.Add("Try to damage my hp by matching the cards.");
        thisFujikoDialogBook.Add("If you have higher hp at the end, you win.");
        thisFujikoDialogBook.Add("And you get your lovely tub back.");
    }

    protected void InitializeGoemonDialogBook()
    {
        thisGoemonDialogBook.Add("Not good until I get what you have stolen from me.");
        thisGoemonDialogBook.Add("My favorite cauldron bathtub. I can't sleep without it.");
        thisGoemonDialogBook.Add("I need it, too. Fujiko, let's be reasonable.");
        thisGoemonDialogBook.Add("...?");
        thisGoemonDialogBook.Add("...");
        thisGoemonDialogBook.Add("...");
        thisGoemonDialogBook.Add("Deal.");
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
                    thisFujikoPrompt.gameObject.SetActive(false);

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
                        //UnityEditor.EditorApplication.isPlaying = false;
                        SceneManager.LoadScene("Loading");
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

                    thisFujikoHasSpoken = true;
                    thisDialogState = State.eWaiting;
                }
            break;

            case State.eGoemonSpeaks:
                {
                    HideFujikoDialog();
                    DisplayGoemonDialog();

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
        //thisFujikoPrompt.gameObject.SetActive(false);
        thisFujikoText.text = " ";
    }

    protected void HideGoemonDialog()
    {
        thisGoemonDialogBox.gameObject.SetActive(false);
        //thisGoemonPrompt.gameObject.SetActive(false);
        thisGoemonText.text = " ";
    }

    protected void DisplayFujikoDialog()
    {
        //thisFujikoPrompt.gameObject.SetActive(true);
        thisFujikoDialogBox.gameObject.SetActive(true);
        thisFujikoText.text = thisFujikoDialogBook[thisCurrentDialogIndex];
    }

    protected void DisplayGoemonDialog()
    {
        thisGoemonDialogBox.gameObject.SetActive(true);
        //thisGoemonPrompt.gameObject.SetActive(true);
        thisGoemonText.text = thisGoemonDialogBook[thisCurrentDialogIndex];
    }
}