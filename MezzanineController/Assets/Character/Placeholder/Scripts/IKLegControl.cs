using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegControl : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Transform rightLegObj = null;
    public Transform lookObj = null;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal
            if (ikActive)
            {
                //set the look target position, if one has been assigned
                if(lookObj != null){
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);}


                //set the right leg position and rotation, if one has been assigned
                if(rightLegObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightLegObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightLegObj.rotation); }
            }

            //if the ik is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetLookAtWeight(0);
            }

            }
    }
}
