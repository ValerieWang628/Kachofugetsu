using UnityEngine;

public class LoadingCircleScript : MonoBehaviour
{

    protected void Update()
    {
        this.transform.eulerAngles += new Vector3(0,0,2f);
    }
}
