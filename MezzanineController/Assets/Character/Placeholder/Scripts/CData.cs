using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CData : MonoBehaviour
{
    public static CData i;

    private void Awake()
    {
        //set Singleton
        if (i != null && i != this)
        {
            Destroy(this);
        }
        else
        {
            i = this;
        }
    }


    }
