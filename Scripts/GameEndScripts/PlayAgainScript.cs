using UnityEngine;

public class PlayAgainScript : MonoBehaviour
{
    [SerializeField] private AudioSource thisBg;
    private AudioManagerScript thisAudioManager;

    private float t = 0f;

    private float thisFadeOutDuration = 0.5f;

    protected void Awake()
    {
        thisAudioManager = (AudioManagerScript)FindObjectOfType(typeof(AudioManagerScript));
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            thisAudioManager.StartFadingOut("Main", thisBg, thisBg.volume, thisFadeOutDuration, t);
        }
    }
}
