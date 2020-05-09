using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Opening");
        }
    }
}
