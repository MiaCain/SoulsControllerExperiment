using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIK : MonoBehaviour
{
    public bool LeftHandActive;
    public bool RightHandActive;
    public Transform targetPos;

    private void OnAnimatorIK()
    {
        if (LeftHandActive)
        {
            InputScript.i.HumanAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            InputScript.i.HumanAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            InputScript.i.HumanAnimator.SetIKPosition(AvatarIKGoal.LeftHand, targetPos.transform.position);
            InputScript.i.HumanAnimator.SetIKRotation(AvatarIKGoal.LeftHand, targetPos.transform.rotation);
        }
    }



    public void StartLeftHandIK()
    {
        LeftHandActive = true;
    }
}
