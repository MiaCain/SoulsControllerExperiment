using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockonCamTurn : MonoBehaviour
{
    private void LateUpdate()
    {
        if (InputScript.i.isLockedOn)
        {
            transform.LookAt(InputScript.i.LockedEnemy.transform, Vector3.up);
        }
    }
}
