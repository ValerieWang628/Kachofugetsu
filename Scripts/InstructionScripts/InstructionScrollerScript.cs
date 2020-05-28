using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionScrollerScript : MonoBehaviour
{
    [SerializeField] private GameObject thisParticles;

    [SerializeField] private List<Sprite> thisInstructionSeries = new List<Sprite>();

    private SpriteRenderer thisSpriteRenderer;

    private int thisInd = 0;
    protected void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
    }


    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (thisInd == thisInstructionSeries.Count -1)
            {
                SceneManager.LoadScene("Loading");
            }
            else
            {
                thisParticles.SetActive(false);
                thisInd++;
                thisSpriteRenderer.sprite = thisInstructionSeries[thisInd];
            }
        }
    }
}
