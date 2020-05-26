using UnityEngine;
using System.Collections.Generic;

public class EyeBlinkingScript : MonoBehaviour
{
    [SerializeField] private GameObject theSelfDialog;
    private InGameDialogScript thisSelfDialog;

    [SerializeField] private Sprite thisEyeOpenSprite;
    [SerializeField] private Sprite thisEyeCloseSprite;
    [SerializeField] private float thisOpenMin;
    [SerializeField] private float thisOpenMax;

    private SpriteRenderer thisEyeSpriteRenderer;

    private float thisTimer;

    private bool thisStateAborted = false;

    [SerializeField] private GameObject thisMask;
    [SerializeField] private GameObject thisOriginalHair;

    private SpriteRenderer thisMaskSprRdr;

    private List<List<Sprite>> thisLookGallery = new List<List<Sprite>>();
    private List<List<Sprite>> thisLookGalleryCopy = new List<List<Sprite>>();

    private int thisCurrentI = 100;

    [SerializeField] private List<Sprite> thisLook0 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook1 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook2 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook3 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook4 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook5 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook6 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook7 = new List<Sprite>();
    [SerializeField] private List<Sprite> thisLook8 = new List<Sprite>();

    enum State
    {
        eOpen,
        eClose,
        eWaiting
    }

    private State eyeState;

    protected void Start()
    {
        thisEyeSpriteRenderer = GetComponent<SpriteRenderer>();
        eyeState = State.eWaiting;
        thisTimer = Random.Range(thisOpenMin, thisOpenMax);
        thisSelfDialog = theSelfDialog.GetComponent<InGameDialogScript>();

        InitializeMasks();
        InitializeLooks();
        InitializeLookCopy();
    }

    protected void InitializeMasks()
    {
        thisMaskSprRdr = thisMask.GetComponent<SpriteRenderer>();
    }

    protected void InitializeLooks()
    {
        thisLookGallery.Add(thisLook0);
        thisLookGallery.Add(thisLook1);
        thisLookGallery.Add(thisLook2);
        thisLookGallery.Add(thisLook3);
        thisLookGallery.Add(thisLook4);
        thisLookGallery.Add(thisLook5);
        thisLookGallery.Add(thisLook6);
        thisLookGallery.Add(thisLook7);
        thisLookGallery.Add(thisLook8);
    }

    protected void InitializeLookCopy()
    {
        thisLookGalleryCopy.Add(thisLook0);
        thisLookGalleryCopy.Add(thisLook1);
        thisLookGalleryCopy.Add(thisLook2);
        thisLookGalleryCopy.Add(thisLook3);
        thisLookGalleryCopy.Add(thisLook4);
        thisLookGalleryCopy.Add(thisLook5);
        thisLookGalleryCopy.Add(thisLook6);
        thisLookGalleryCopy.Add(thisLook7);
        thisLookGalleryCopy.Add(thisLook8);
    }

    protected void Update()
    {
        UpdateSwitch();   
    }

    public void StartHumiliation()
    {
        if (thisCurrentI < thisLookGallery.Count)
        {
            // remove this current one
            thisLookGallery.RemoveAt(thisCurrentI);
        }
        // fetch a new rand ind
        thisCurrentI = Random.Range(0, thisLookGallery.Count);

        // call dialog to update text
        //foreach (List<Sprite> aSpriteList in thisLookGalleryCopy)
        //{
        //    if (thisLookGallery[thisCurrentI] == aSpriteList)
        //    {

        //    }
        //}
        for (int i = 0; i < thisLookGalleryCopy.Count; i++)
        {
            if (thisLookGallery[thisCurrentI] == thisLookGalleryCopy[i])
            {
                thisSelfDialog.DisplayDialog(i);
                return;
            }
        }
    }

    protected void UpdateSwitch()
    {
        switch (eyeState)
        {
            case State.eWaiting:
                {
                    if (thisStateAborted)
                    {
                        thisStateAborted = false;
                        return;
                    }

                    thisTimer -= Time.deltaTime;

                    if (thisTimer < 0)
                    {
                        if (thisEyeSpriteRenderer.sprite == thisEyeOpenSprite)
                        {
                            eyeState = State.eClose;
                            thisTimer = Random.Range(0.15f, thisOpenMin);
                            return;
                        }
                        else
                        {
                            eyeState = State.eOpen;
                            thisTimer = Random.Range(thisOpenMin, thisOpenMax);
                            return;
                        }
                    }
                }
                break;

            case State.eOpen:
                {
                    eyeState = State.eWaiting;
                    thisEyeSpriteRenderer.sprite = thisEyeOpenSprite;

                    if (thisCurrentI < thisLookGallery.Count)
                    {
                        ChangeFace(true);
                    }

                    return;
                }

            case State.eClose:
                {
                    eyeState = State.eWaiting;
                    thisEyeSpriteRenderer.sprite = thisEyeCloseSprite;

                    if (thisCurrentI < thisLookGallery.Count)
                    {
                        ChangeFace(false);
                    }

                    return;
                }
        }
    }

    protected void ChangeFace(bool eyeIsOpen)
    {
        List<Sprite> currentLook = thisLookGallery[thisCurrentI];

        // disable previous hair
        thisOriginalHair.SetActive(false);


        if (currentLook.Count == 1)
        {
            thisMaskSprRdr.sprite = currentLook[0];
            return;
        }
        else
        {
            if (eyeIsOpen)
            {
                thisMaskSprRdr.sprite = currentLook[0];
                return;
            }
            else
            {
                thisMaskSprRdr.sprite = currentLook[1];
                return;
            }
        }
    }

    public void AbortState()
    {
        thisStateAborted = true;
        eyeState = State.eOpen;
    }

}
