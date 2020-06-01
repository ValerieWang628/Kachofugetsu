using UnityEngine;

public class FrameRateScript : MonoBehaviour
{
    [SerializeField] private int thisFps;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = thisFps;
    }


    void Update()
    {
        
    }
}
