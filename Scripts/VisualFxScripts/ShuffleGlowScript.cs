using System.Collections;
using UnityEngine;

public class ShuffleGlowScript : MonoBehaviour
{

    private SpriteRenderer thisSpriteRenderer;

    protected void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();

        // set full transparency
        Color theColor = thisSpriteRenderer.color;
        theColor.a = 0f;
        thisSpriteRenderer.color = theColor;
    }

    protected IEnumerator FadeInOut()
    {
        while (thisSpriteRenderer.color.a <= 1f)
        {
            float theFadingFrequency = 0.01f;
            Color theColor = thisSpriteRenderer.color;
            theColor.a += theFadingFrequency;
            thisSpriteRenderer.color = theColor;
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        while (thisSpriteRenderer.color.a >= 0f)
        {
            float theFadingFrequency = 0.01f;
            Color theColor = thisSpriteRenderer.color;
            theColor.a -= theFadingFrequency;
            thisSpriteRenderer.color = theColor;
            yield return null;
        }
        yield break;
    }

    public void StartToFadeInOut()
    {
        StartCoroutine(FadeInOut());
    }
}
