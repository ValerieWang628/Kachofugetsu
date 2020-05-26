// this script is currently abandoned

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticsManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject thisGoemonMask;
    [SerializeField] private GameObject thisFujikoMask;

    private SpriteRenderer thisGoemonSprRdr;
    private SpriteRenderer thisFujikoSprRdr;

    [SerializeField] private Sprite thisGoemonKissOpen;
    [SerializeField] private Sprite thisGoemonKissClosed;

    protected void Start()
    {
        thisGoemonSprRdr = thisGoemonMask.GetComponent<SpriteRenderer>();
        //thisFujikoSprRdr = thisFujikoMask.GetComponent<SpriteRenderer>();
    }


    protected void Update()
    {
        
    }

    public void ChangeGoemonFace(bool eyeIsOpen, string theCosmeticLook)
    {
        switch (theCosmeticLook)
        {
            case "Kiss":
                {
                    if (eyeIsOpen)
                    {
                        thisGoemonSprRdr.sprite = thisGoemonKissOpen;
                        return;
                    }
                    else
                    {
                        thisGoemonSprRdr.sprite = thisGoemonKissClosed;
                        return;
                    }

                }
        }

    }
}
