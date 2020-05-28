using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameDialogScript : MonoBehaviour
{
    private float thisTimer;
    private float thisDuration = 4.5f;

    [SerializeField] Text thisSelfDialog;

    [SerializeField] string thisMoonLine;
    [SerializeField] string thisBirdLine;
    [SerializeField] string thisWindLine;
    [SerializeField] string thisFlowerLine;

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

    public void DisplayMoonLine()
    {
        DisplaySpecialLine(thisMoonLine);
    }

    public void DisplayFlowerLine()
    {
        DisplaySpecialLine(thisFlowerLine);
    }

    public void DisplayWindLine()
    {
        DisplaySpecialLine(thisWindLine);
    }

    public void DisplayBirdLine()
    {
        DisplaySpecialLine(thisBirdLine);
    }

    protected void DisplaySpecialLine(string aLine)
    {
        thisSelfDialog.gameObject.SetActive(true);
        thisSelfDialog.text = aLine;
    }
}
