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
    }

    protected void InitializeMasks()
    {
        thisMaskSprRdr = thisMask.GetComponent<SpriteRenderer>();
    }

    protected void InitializeLooks()
    {
        List<Sprite>[] aSpriteList = {thisLook0, thisLook1, thisLook2, thisLook3, thisLook4, 
                                        thisLook5, thisLook6, thisLook7, thisLook8};

        thisLookGallery = new List<List<Sprite>>(aSpriteList);
        thisLookGalleryCopy = new List<List<Sprite>>(aSpriteList);
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
