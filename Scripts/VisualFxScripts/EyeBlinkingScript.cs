using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlinkingScript : MonoBehaviour
{

    [SerializeField] private Sprite thisEyeOpenSprite;
    [SerializeField] private Sprite thisEyeCloseSprite;
    [SerializeField] private float thisOpenMin;
    [SerializeField] private float thisOpenMax;

    private SpriteRenderer thisSpriteRenderer;

    private float thisTimer;

    enum State
    {
        eOpen,
        eClose,
        eWaiting
    }

    private State eyeState;

    protected void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        eyeState = State.eWaiting;
        thisTimer = Random.Range(thisOpenMin, thisOpenMax);
    }


    protected void Update()
    {
        UpdateSwitch();   
    }

    protected void UpdateSwitch()
    {
        switch (eyeState)
        {
            case State.eWaiting:
                {
                    thisTimer -= Time.deltaTime;

                    if (thisTimer < 0)
                    {
                        if (thisSpriteRenderer.sprite == thisEyeOpenSprite)
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
                    thisSpriteRenderer.sprite = thisEyeOpenSprite;
                    eyeState = State.eWaiting;
                    return;
                }

            case State.eClose:
                {
                    thisSpriteRenderer.sprite = thisEyeCloseSprite;
                    eyeState = State.eWaiting;
                    return;
                }
        }
    }

}
