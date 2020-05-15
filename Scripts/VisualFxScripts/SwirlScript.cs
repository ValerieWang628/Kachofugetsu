using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirlScript : MonoBehaviour
{
    private SpriteRenderer thisSpriteRenderer;

    //private float thisFadingFreq = 0.01f;

    protected void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();

        Color theColor = thisSpriteRenderer.color;
        theColor.a = 0f;
        thisSpriteRenderer.color = theColor;

    }

    public void Manifest()
    {
        StartCoroutine(Fading());
        StartCoroutine(Rotate());
        StartCoroutine(Shrink());
    }

    protected IEnumerator Fading()
    {
        while (thisSpriteRenderer.color.a <= 1f)
        {
            float theFadingFrequency = 0.01f;
            Color theColor = thisSpriteRenderer.color;
            theColor.a += theFadingFrequency;
            thisSpriteRenderer.color = theColor;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        while (thisSpriteRenderer.color.a >= 0f)
        {
            float theFadingFrequency = 0.005f;
            Color theColor = thisSpriteRenderer.color;
            theColor.a -= theFadingFrequency;
            thisSpriteRenderer.color = theColor;
            //print(thisSpriteRenderer.color.a + "\n");
            yield return null;
        }
        yield break;
    }

    protected IEnumerator Rotate() 
    {
        while (true)
        {
            this.transform.eulerAngles += new Vector3(0f, 0f, 2f);
            yield return null;
        }
    }


    protected IEnumerator Shrink()
    {
        yield return new WaitForSeconds(1.5f);

        while (this.transform.localScale.x > 0.01f)
        {
            this.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            yield return null;
        }
        yield break;
    }

}
