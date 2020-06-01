using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionScrollerScript : MonoBehaviour
{
    [SerializeField] private AudioSource thisBg;
    private AudioManagerScript thisAudioManager;

    private float t = 0f;
    private float thisFadeOutDuration = 1f;

    [SerializeField] private GameObject thisParticles;

    [SerializeField] private List<Sprite> thisInstructionSeries = new List<Sprite>();

    private SpriteRenderer thisSpriteRenderer;

    private int thisInd = 0;

    protected void Awake()
    {
        // finding objs from dontdestroy requires starting from the first scene
        thisAudioManager = (AudioManagerScript)FindObjectOfType(typeof(AudioManagerScript));
    }

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
                thisAudioManager.StartFadingOut("Loading", thisBg, thisBg.volume, thisFadeOutDuration, t);
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
