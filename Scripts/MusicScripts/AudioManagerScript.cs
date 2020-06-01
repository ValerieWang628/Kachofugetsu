using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    private static AudioManagerScript instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    protected IEnumerator FadeOut(string theScene, AudioSource theBg, float theMaxVol, float theDuration, float t)
    {
        while (t < theDuration)
        {
            t += Time.deltaTime;
            theBg.volume = Mathf.Lerp(theMaxVol, 0f, t / theDuration);
            yield return null;
        }

        SceneManager.LoadScene(theScene);
        yield break;
    }

    public void StartFadingOut(string theScene, AudioSource theBg, float theMaxVol, float theDuration, float t)
    {
        StartCoroutine(FadeOut(theScene, theBg, theMaxVol, theDuration, t));
    }

}
