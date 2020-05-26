using System.Collections;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    [SerializeField] private GameObject thisEndDot;
    private TrailRenderer thisTrailRenderer;
    private Vector3 thisOriginalPos;
    private Vector3 thisEndPos;
    private IEnumerator thisSlashCoroutine;

    private float t = 0f;
    private float thisSlashDuration = 0.3f;

    protected void Start()
    {
        thisTrailRenderer = this.GetComponent<TrailRenderer>();

        thisOriginalPos = this.transform.position;

        thisEndPos = thisEndDot.transform.position;
    }

    public void StartSlash()
    {
        t = 0f;
        this.transform.position = thisOriginalPos;
        thisSlashCoroutine = Slash();
        StartCoroutine(thisSlashCoroutine);
    }


    public void StopSlash()
    {
        StopCoroutine(thisSlashCoroutine);
    }

    protected IEnumerator Slash()
    {
        while (t < thisSlashDuration)
        {
            t += Time.deltaTime;

            this.transform.position = Vector3.Lerp(thisOriginalPos, thisEndPos, t / thisSlashDuration);
            yield return null;
        }

        yield break;
    }

    public bool GetCoroutineStatus()
    {
        if (thisSlashCoroutine != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected IEnumerator SlashOverTime()
    {
        /* a test func to test the slash*/
        while (true)
        {
            StartSlash();
            yield return null;

            yield return new WaitForSeconds(3f);

        }
    }

}
