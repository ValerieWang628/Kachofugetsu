using UnityEngine;

public class StartScript : MonoBehaviour
{
    [SerializeField] private AudioSource thisAmbientBg;

    private float t = 0f;

    private float thisFadeOutDuration = 0.7f;

    [SerializeField] private GameObject theAudioManager;
    private AudioManagerScript thisAudioManager;

    protected void Start()
    {
        //thisMaxVolume = thisAmbientBg.volume;
        thisAudioManager = theAudioManager.GetComponent<AudioManagerScript>();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            thisAudioManager.StartFadingOut("Opening", thisAmbientBg, thisAmbientBg.volume, thisFadeOutDuration, t);
        }
    }
   
}
