using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{

    private PlayerBehaviorScript thisPlayerBehaviorScript;

    // the glow frame outside the card
    private GameObject thisGlow;

    private float thisRotationIncrement = 1.2f;

    private bool thisHasBeenFlipped = false;

    private IEnumerator thisFlipCoroutine;

    private IEnumerator thisFlipBackCoroutine;

    private IEnumerator thisLerpToShuffleCoroutine;

    private bool thisShuffleHasEnded = false;

    // this is a bool to control the gate of emission
    // the permission to emit cannot depend on thisHasBeenFlipped because of weird coroutine
    private bool thisEmittable = true;

    protected void Start()
    {
        FindPlayerBehavior();
        InitializeGlow();
    }

    protected void FindPlayerBehavior()
    {
        thisPlayerBehaviorScript = GameObject.Find("PlayerBehavior").GetComponent<PlayerBehaviorScript>();
    }

    protected void InitializeGlow()
    {
        thisGlow = this.transform.GetChild(2).gameObject;
        thisGlow.SetActive(false);
    }

    protected void Update()
    {

    }

    public void MakeEmittable()
    {
        thisEmittable = true;
    }

    protected void OnMouseOver()
    {
        if (!thisHasBeenFlipped)
        {
            if (thisEmittable)
            {
                EmitGlow();
            }
        }
    }

    protected void OnMouseUp()
    {
        /* only if the player is at a state where s/he can click
         * and if the card has not been flipped
         * can this card be flipped
         */

        thisEmittable = !thisEmittable;

        bool thePlayerCanClick = thisPlayerBehaviorScript.GetIfThePlayerCanClick();

        if (thePlayerCanClick)
        {
            if (!thisHasBeenFlipped)
            {

                // pass the crad itself to player selection list
                thisPlayerBehaviorScript.OnCardSelected(this.gameObject);

                StartFlipCoroutine();
            }
        }
    }

    protected void OnMouseExit()
    {

        if (!thisHasBeenFlipped)
        {
            OmitGlow();
        }
    }

    public void StartFlipCoroutine()
    {
        thisFlipCoroutine = Flip();
        StartCoroutine(thisFlipCoroutine);
    }

    protected IEnumerator Flip()
    {
        /* an animation coroutine
         * to flip the card from content covered to content revealed
         */

        while (this.transform.eulerAngles.y < 180f)
        {
            //print(this.name + " flip " + this.transform.eulerAngles.y + "\n");
            this.transform.eulerAngles += new Vector3(0f, thisRotationIncrement, 0f);
            yield return null;
        }

        // make it unflippable
        thisHasBeenFlipped = true;
        yield break;
    }

    public void StartFlipBackCoroutine()
    {
        thisFlipBackCoroutine = FlipBack();
        StartCoroutine(thisFlipBackCoroutine);
    }

    protected IEnumerator FlipBack()
    {
        /* an animation coroutine
         * to flip the card from content revealed to content covered
         */

        /* the reason why the threshold here is 1 is that 
         * when the angles keeps decreasing until it gets close to zero
         * the angles won't get negative but to wrap back to 360
         * So the flip animation will go on forever
         * the solution is to raise the threshold a bit so that it stops right at time
         */

        this.thisHasBeenFlipped = false;
        yield return null;

        while (this.transform.eulerAngles.y >= 1f)
        {
            //print(this.name + " flip back " + this.transform.eulerAngles.y + "\n");
            this.transform.eulerAngles -= new Vector3(0f, thisRotationIncrement, 0f);
            yield return null;
        }

        // set the bool back so that it can be re-clicked
        //this.thisHasBeenFlipped = false;
        yield break;
    }

    public void StartLerpToShuffleCoroutine(Vector3 theShufflePointPos, Vector3 theShuffledDestination, float thePercentage)
    {
        thisLerpToShuffleCoroutine = LerpToShuffle(theShufflePointPos, theShuffledDestination, thePercentage);
        StartCoroutine(thisLerpToShuffleCoroutine);
    }

    protected IEnumerator LerpToShuffle(Vector3 theShufflePointPos, Vector3 theShuffledDestination,float thePercentage)
    {
        while (this.transform.position != theShufflePointPos)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, theShufflePointPos, thePercentage);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        while (this.transform.position != theShuffledDestination)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, theShuffledDestination, thePercentage);
            yield return null;
        }

        thisShuffleHasEnded = true;
        yield break;
    }

    protected void EmitGlow()
    {
        /* this func makes the glowing frame visible*/
        thisGlow.SetActive(true);
    }

    protected void OmitGlow()
    {
        /* this func makes the glowing frame invisible*/
        thisGlow.SetActive(false);
    }

    public void StopLerpToShuffleCoroutine()
    {
        StopCoroutine(thisLerpToShuffleCoroutine);
    }

    public void StopFlipCoroutine()
    {
        StopCoroutine(thisFlipCoroutine);
    }


    public bool GetIfCardHasBeenFlipped()
    { 
        /* a func that returns if this card has been flipped*/
        return thisHasBeenFlipped;
    }

    public bool GetIfShuffleComplete()
    {
        return thisShuffleHasEnded;
    }
}
