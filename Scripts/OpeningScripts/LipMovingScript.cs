using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipMovingScript : MonoBehaviour
{

    [SerializeField] private Sprite thisLipOpenSprite;
    [SerializeField] private Sprite thisLipCloseSprite;
    [SerializeField] private float thisOpenMin;
    [SerializeField] private float thisOpenMax;

    private SpriteRenderer thisSpriteRenderer;

    private float thisTimer;

    private bool thisLipCanMove;

    enum State
    {
        eOpen,
        eClose,
        eWaiting
    }

    private State lipState;


    protected void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        lipState = State.eWaiting;
        thisTimer = Random.Range(thisOpenMin, thisOpenMax);
    }


    protected void Update()
    {
        UpdateSwitch();
    }

    protected void UpdateSwitch()
    {
        switch (lipState)
        {
            case State.eWaiting:
                {
                    if (thisLipCanMove)
                    {
                        thisTimer -= Time.deltaTime;

                        if (thisTimer < 0)
                        {
                            if (thisSpriteRenderer.sprite == thisLipOpenSprite)
                            {
                                lipState = State.eClose;
                                thisTimer = Random.Range(0f, thisOpenMin);
                                return;
                            }
                            else
                            {
                                lipState = State.eOpen;
                                thisTimer = Random.Range(thisOpenMin, thisOpenMax);
                                return;
                            }
                        }
                    }
                }
                break;

            case State.eOpen:
                {
                    thisSpriteRenderer.sprite = thisLipOpenSprite;
                    lipState = State.eWaiting;
                    return;
                }

            case State.eClose:
                {
                    thisSpriteRenderer.sprite = thisLipCloseSprite;
                    lipState = State.eWaiting;
                    return;
                }
        }
    }

    public void StartTalking()
    {
        thisLipCanMove = true;
    }

    public void StopTalking()
    {
        thisLipCanMove = false;
    }
}
