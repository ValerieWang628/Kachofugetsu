using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingWaitScript : MonoBehaviour
{
    private float thisTimer;
    private float thisDuration = 3f;

    protected void Start()
    {
        thisTimer = thisDuration;
    }


    protected void Update()
    {
        thisTimer -= Time.deltaTime;

        if (thisTimer < 0)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
