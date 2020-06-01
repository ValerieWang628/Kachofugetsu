using UnityEngine;

public class AmbientScript : MonoBehaviour
{
    private static AmbientScript instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
