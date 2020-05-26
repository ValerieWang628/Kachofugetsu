using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameDialogScript : MonoBehaviour
{
    private float thisTimer;
    private float thisDuration = 3f;

    [SerializeField] Text thisSelfDialog;

    [SerializeField] List<string> thisDialogList = new List<string>();

    protected void Start()
    {
        SetTimer();
    }

    protected void Update()
    {
        if (this.gameObject.activeSelf)
        {
            thisTimer -= Time.deltaTime;
        }

        if (thisTimer < 0)
        {
            SetTimer();
            this.gameObject.SetActive(false);
            thisSelfDialog.gameObject.SetActive(false);
        }
    }

    protected void SetTimer()
    {
        thisTimer = thisDuration;
    }

    public void DisplayDialog(int i)
    {
        thisSelfDialog.gameObject.SetActive(true);
        thisSelfDialog.text = thisDialogList[i];
    }
}
