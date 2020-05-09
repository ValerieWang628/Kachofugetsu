using UnityEngine;
using UnityEngine.UI;

public class UiManagerScript : MonoBehaviour
{
    [SerializeField] private Text thisNpcHpText;
    [SerializeField] private Text thisPlayerHpText;
    [SerializeField] private Text thisTurnPrompt;

    // use the not this is because it won't be used much
    // so it won't interfere with auto completion
    [SerializeField] private GameObject theNpc;
    [SerializeField] private GameObject thePlayer;


    private NpcBehaviorScript thisNpcBehavior;
    private PlayerBehaviorScript thisPlayerBehavior;

    protected void Start()
    {
        InitializeData();

        UpdateNpcText();
        UpdatePlayerText();
    }

    protected void InitializeData()
    {
        thisNpcBehavior = theNpc.GetComponent<NpcBehaviorScript>();
        thisPlayerBehavior = thePlayer.GetComponent<PlayerBehaviorScript>();
    }

    public void UpdateNpcText()
    {
        thisNpcHpText.text = thisNpcBehavior.GetNpcHitPoints().ToString();
    }

    public void UpdatePlayerText()
    {
        thisPlayerHpText.text = thisPlayerBehavior.GetPlayerHitPoints().ToString();
    }

    public void UpdatePlayerTurn()
    {
        thisTurnPrompt.text = "Goemon";
    }

    public void UpdateNpcTurn()
    {
        thisTurnPrompt.text = "Fujiko";
    }

    public void UpdateShuffling()
    {
        thisTurnPrompt.text = "Shuffling";
    }


    protected void Update()
    {
        
    }
}
